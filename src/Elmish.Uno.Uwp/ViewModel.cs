using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Elmish.Uno
{
    /// <summary>
    /// Implementation of dynamic property required by WinRT to do bindings.
    /// </summary>
    /// <typeparam name="TTarget">Target object type from which to get and to which to set a property value.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    public class DynamicCustomProperty<TTarget, TValue> : ICustomProperty
    {
        /// <summary>
        /// Property getter delegate.
        /// </summary>
        public Func<TTarget, TValue> Getter { get; }
        /// <summary>
        /// Property setter delegate
        /// </summary>
        public Action<TTarget, TValue> Setter { get; }
        /// <summary>
        /// Indexer getter delegate
        /// </summary>
        public Func<TTarget, object, TValue> IndexGetter { get; }
        /// <summary>
        /// Indexer setter delegate
        /// </summary>
        public Action<TTarget, object, TValue> IndexSetter { get; }

        /// <summary>Gets the value of the custom property from a particular instance.</summary>
        /// <param name="target">The owning instance.</param>
        /// <returns>The retrieved value.</returns>
        public object GetValue(object target) => Getter.Invoke((TTarget)target);
        /// <summary>Sets the custom property value on a specified instance.</summary>
        /// <param name="target">The owner instance.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(object target, object value) => Setter.Invoke((TTarget)target, (TValue)value);
        /// <summary>Gets the value at an index location, for cases where the custom property has indexer support.</summary>
        /// <param name="target">The owning instance.</param>
        /// <param name="index">The index to get.</param>
        /// <returns>The retrieved value at the index.</returns>
        public object GetIndexedValue(object target, object index) => IndexGetter.Invoke((TTarget)target, index);
        /// <summary>Sets the value at an index location, for cases where the custom property has indexer support.</summary>
        /// <param name="target">The owner instance.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="index">The index location to set to.</param>
        public void SetIndexedValue(object target, object value, object index) => IndexSetter.Invoke((TTarget)target, index, (TValue)value);

        /// <summary>Gets a value that determines whether the custom property supports read access.</summary>
        /// <returns>**true** if the property value can be read as a data source. **false** if the property cannot be a data source value.</returns>
        public bool CanRead => Getter != null || IndexGetter != null;
        /// <summary>Gets a value that determines whether the custom property supports write access.</summary>
        /// <returns>**true** if the value can be written to through a data source relationship in a two-way binding. **false** if the property cannot be written to.</returns>
        public bool CanWrite => Setter != null || IndexSetter != null;
        /// <summary>Gets the path-relevant name of the property.</summary>
        /// <returns>The name of the property as it would be specified in a binding expression.</returns>
        public string Name { get; }
        /// <summary>Gets the underlying type of the custom property.</summary>
        /// <returns>The underlying type, with relevant information as the values of the TypeName structure. TypeName provides the infrastructure such that property backing does not have to resemble common language runtime (CLR) and **System.Type** definitions.</returns>
        public Type Type => typeof(TValue);

        /// <summary>
        /// Creates an instance of <see href="DynamicCustomProperty">DynamicCustomProperty</see>.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="getter">Property getter delegate.</param>
        /// <param name="setter">Property setter delegate.</param>
        /// <param name="indexGetter">Indexer getter delegate.</param>
        /// <param name="indexSetter">Indexer setter delegate.</param>
        public DynamicCustomProperty(string name, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter = null, Func<TTarget, object, TValue> indexGetter = null, Action<TTarget, object, TValue> indexSetter = null)
        {
            Name = name;
            Getter = getter;
            Setter = setter;
            IndexGetter = indexGetter;
            IndexSetter = indexSetter;
        }
    }

    internal class ViewModel<TModel, TMsg> : ViewModelBase<TModel, TMsg>, ICustomPropertyProvider
    {
        private readonly ImmutableDictionary<string, string> bindingsMap;

        public ViewModel(TModel initialModel, FSharpFunc<TMsg, Unit> dispatch, FSharpList<Binding<TModel, TMsg>> bindings, ElmConfig config, string propNameChain) : base(initialModel, dispatch, bindings, config, propNameChain)
        {
            string GetBindingType(Binding<TModel, TMsg> binding)
            {
                //var (info, _) = FSharpValue.GetUnionFields(binding.Data, typeof(BindingData<TModel, TMsg>), FSharpOption<BindingFlags>.Some(BindingFlags.NonPublic));
                return binding.Data.GetType().Name;
            }

            this.bindingsMap = bindings.ToImmutableDictionary(b => b.Name, GetBindingType);
        }

        public override ViewModelBase<TSubModel, TSubMsg> Create<TSubModel, TSubMsg>(TSubModel initialModel, FSharpFunc<TSubMsg, Unit> dispatch, FSharpList<Binding<TSubModel, TSubMsg>> bindings, ElmConfig config, string propNameChain)
         => new ViewModel<TSubModel, TSubMsg>(initialModel, dispatch, bindings, config, propNameChain);

        public override ObservableCollection<T> CreateCollection<T>(FSharpFunc<TModel, bool> hasMoreItems, FSharpFunc<Tuple<uint, FSharpFunc<uint, Unit>>, TMsg> loadMoreitems, System.Collections.Generic.IEnumerable<T> collection)
        {
            void LoadMoreitems(uint count, FSharpFunc<uint, Unit> complete)
            {
                var msg = loadMoreitems.Invoke(new Tuple<uint, FSharpFunc<uint, Unit>>(count, complete));
                Dispatch.Invoke(msg);
            }
            return new IncrementalLoadingCollection<T>(collection, () => hasMoreItems.Invoke(this.currentModel), LoadMoreitems);
        }


        private ICustomProperty GetProperty(string name)
        {
            if (name == "CurrentModel") return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.CurrentModel);
            if (name == "HasErrors") return new DynamicCustomProperty<ViewModel<TModel, TMsg>, bool>(name, vm => ((INotifyDataErrorInfo)vm).HasErrors);
            if (!bindingsMap.TryGetValue(name, out var bindingType)) Debugger.Break();
            switch (bindingType)
            {
                case nameof(BindingData<TModel, TMsg>.OneWayData):
                case nameof(BindingData<TModel, TMsg>.OneWayLazyData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.OneWaySeqLazyData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ObservableCollection<object>>(name,
                        vm => (ObservableCollection<object>)vm.TryGetMember(vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.TwoWayData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name,
                        vm => vm.TryGetMember(vm.Bindings[name]), (vm, value) => vm.TrySetMember(value, vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.TwoWayValidateData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name,
                        vm => vm.TryGetMember(vm.Bindings[name]), (vm, value) => vm.TrySetMember(value, vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.CmdData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ICommand>(name,
                        vm => vm.TryGetMember(vm.Bindings[name]) as ICommand);
                case nameof(BindingData<TModel, TMsg>.CmdParamData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.SubModelData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ViewModel<object, object>>(name,
                        vm => vm.TryGetMember(vm.Bindings[name]) as ViewModel<object, object>);
                case nameof(BindingData<TModel, TMsg>.SubModelSeqData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ObservableCollection<Uno.ViewModel<object, object>>>(name,
                        vm => (ObservableCollection<Uno.ViewModel<object, object>>)vm.TryGetMember(vm.Bindings[name]));
                case nameof(BindingData<TModel, TMsg>.SubModelSelectedItemData):
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ViewModel<object, object>>(name,
                        vm => (ViewModel<object, object>)vm.TryGetMember(vm.Bindings[name]));
                default:
                    return null;
                    //throw new NotSupportedException();
            }
        }

        public ICustomProperty GetCustomProperty(string name) => GetProperty(name);

        public ICustomProperty GetIndexedProperty(string name, Type type) => GetProperty(name);

        public string GetStringRepresentation() => CurrentModel.ToString();

        public Type Type => CurrentModel.GetType();
    }
}

/// <summary>
/// View model methods to correspond to F# module
/// </summary>
[RequireQualifiedAccess, CompilationMapping(SourceConstructFlags.Module)]
public static class ViewModel
{
    /// <summary>
    /// Creates an instance of Elmish view model without dispatcher subscription.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TMsg">Elmish message type.</typeparam>
    /// <param name="model">Design time model.</param>
    /// <param name="bindings">Elmish program to run.</param>
    /// <returns>An instance of internal Elmish view model class.</returns>
    public static object DesignInstance<TModel, TMsg>(TModel model, FSharpList<Binding<TModel, TMsg>> bindings)
    {
        var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
        return new ViewModel<TModel, TMsg>(model, emptyDispatch, bindings, ElmConfig.Default, "main");
    }

    /// <summary>
    /// Creates an instance of Elmish view model without dispatcher subscription.
    /// and with an initial program argument.
    /// </summary>
    /// <typeparam name="T">Initial program argument type.</typeparam>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TMsg">Elmish message type.</typeparam>
    /// <param name="model">Design time model.</param>
    /// <param name="bindings">Elmish program to run.</param>
    /// <returns>An instance of internal Elmish view model class.</returns>
    public static object DesignInstance<T, TModel, TMsg>(TModel model, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> bindings)
    {
        var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
        var mapping = FSharpFunc<TModel, FSharpFunc<TMsg, Unit>>.InvokeFast(ProgramModule.view(bindings), model, emptyDispatch);
        return DesignInstance(model, mapping);
    }

    /// <summary>
    /// Creates an instance of Elmish view model with dispatcher subscription.
    /// </summary>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TMsg">Elmish message type.</typeparam>
    /// <param name="config">Elmish config.</param>
    /// <param name="element">UI control to set DataContext property on.</param>
    /// <param name="bindings">Elmish bindings definitions list.</param>
    /// <param name="program">Elmish program to run.</param>
    public static void StartLoop<TModel, TMsg>(ElmConfig config, FrameworkElement element, Action<Program<Microsoft.FSharp.Core.Unit, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>>> bindings, Program<Unit, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program)
    {
        FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel = new FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>>(null);
        FSharpFunc<FSharpFunc<TMsg, Unit>, FSharpFunc<TMsg, Unit>> syncDispatch =
          FuncConvert.FromAction(MakeSyncDispatch<TMsg>(element));
        var setState = FuncConvert.FromAction(MakeSetState(config, element, program, lastModel));
        bindings.Invoke(
            ProgramModule.withSyncDispatch(syncDispatch,
              ProgramModule.withSetState(setState, program)));
    }

    /// <summary>
    /// Creates an instance of Elmish view model with dispatcher subscription
    /// and with an initial program argument.
    /// </summary>
    /// <typeparam name="T">Initial program argument type.</typeparam>
    /// <typeparam name="TModel">Model type.</typeparam>
    /// <typeparam name="TMsg">Elmish message type.</typeparam>
    /// <param name="config">Elmish config.</param>
    /// <param name="element">UI control to set DataContext property on.</param>
    /// <param name="bindings">Elmish bindings definitions list.</param>
    /// <param name="program">Elmish program to run.</param>
    /// <param name="arg">Initial program argument.</param>
    public static void StartLoop<T, TModel, TMsg>(ElmConfig config, FrameworkElement element, Action<T, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>>> bindings, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program, T arg)
    {
        FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel = new FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>>(null);
        FSharpFunc<FSharpFunc<TMsg, Unit>, FSharpFunc<TMsg, Unit>> syncDispatch =
          FuncConvert.FromAction(MakeSyncDispatch<TMsg>(element));
        var setState = FuncConvert.FromAction(MakeSetState(config, element, program, lastModel));

        bindings.Invoke(arg,
            ProgramModule.withSyncDispatch(syncDispatch,
              ProgramModule.withSetState(setState, program)));
    }


    private static Action<TModel, FSharpFunc<TMsg, Unit>> MakeSetState<TArg, TModel, TMsg>(ElmConfig config, FrameworkElement element, Program<TArg, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program, FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel)
    {
        void SetState(TModel model, FSharpFunc<TMsg, Unit> dispatch)
        {
            FSharpOption<ViewModel<TModel, TMsg>> contents = lastModel.contents;
            if (contents != null)
            {
                contents.Value.UpdateModel(model);
                return;
            }
            var bindedModel = ProgramModule.view(program).Invoke(model);
            var Bindings = bindedModel.Invoke(dispatch);
            var viewModel = new ViewModel<TModel, TMsg>(model, dispatch, Bindings, config, "main");
            element.DataContext = viewModel;
            lastModel.contents = FSharpOption<ViewModel<TModel, TMsg>>.Some(viewModel);
        }
        return SetState;
    }

    private static Action<FSharpFunc<TMsg, Unit>, TMsg> MakeSyncDispatch<TMsg>(FrameworkElement element)
    {
        void UiDispatch(FSharpFunc<TMsg, Unit> innerDispatch, TMsg msg)
        {
            void DoDispatch(TMsg m)
            {
                Console.WriteLine("Dispatch");
                innerDispatch.Invoke(m);
            }

            _ = element.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DoDispatch(msg));
        }

        return UiDispatch;
    }
}
}
