using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

using Elmish.Uno;

using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Elmish.Windows
{
    public class DynamicCustomProperty<TTarget, TValue> : ICustomProperty
    {
        public Func<TTarget, TValue> Getter { get; }
        public Action<TTarget, TValue> Setter { get; }
        public Func<TTarget, object, TValue> IndexGetter { get; }
        public Action<TTarget, object, TValue> IndexSetter { get; }

        public object GetValue(object target) => Getter.Invoke((TTarget) target);
        public void SetValue(object target, object value) => Setter.Invoke((TTarget)target, (TValue)value);
        public object GetIndexedValue(object target, object index) => IndexGetter.Invoke((TTarget)target, index);
        public void SetIndexedValue(object target, object value, object index) => IndexSetter.Invoke((TTarget)target, index, (TValue)value);

        public bool CanRead => Getter != null || IndexGetter != null;
        public bool CanWrite => Setter != null || IndexSetter != null;
        public string Name { get; }
        public Type Type => typeof(TValue);

        public DynamicCustomProperty(string name, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter = null, Func<TTarget, object, TValue> indexGetter = null, Action<TTarget, object, TValue> indexSetter = null)
        {
            Name = name;
            Getter = getter;
            Setter = setter;
            IndexGetter = indexGetter;
            IndexSetter = indexSetter;
        }
    }

    internal class ViewModel<TModel, TMsg> : Uno.ViewModel<TModel, TMsg>, ICustomPropertyProvider
    {
        public ViewModel(TModel initialModel, FSharpFunc<TMsg, Unit> dispatch, FSharpList<Binding<TModel, TMsg>> bindings, ElmConfig config, string propNameChain) : base(initialModel, dispatch, bindings, config, propNameChain) { }

        public override Uno.ViewModel<object, object> Create(object initialModel, FSharpFunc<object, Unit> dispatch, FSharpList<Binding<object, object>> bindings, ElmConfig config, string propNameChain)
         => new ViewModel<object, object>(initialModel, dispatch, bindings, config, propNameChain);

        private ICustomProperty GetProperty(string name)
        {
            if (name == "CurrentModel") return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.CurrentModel);
            if (name == "HasErrors") return new DynamicCustomProperty<ViewModel<TModel, TMsg>, bool>(name, vm => ((INotifyDataErrorInfo)vm).HasErrors);
            if (!this.Bindings.TryGetValue(name, out var binding)) Debugger.Break();
            switch (binding)
            {
                case VmBinding<TModel, TMsg>.OneWay oneWay:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(oneWay));
                case VmBinding<TModel, TMsg>.OneWayLazy oneWayLazy:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(oneWayLazy));
                case VmBinding<TModel, TMsg>.OneWaySeq oneWaySeq:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ObservableCollection<object>>(name,
                        vm => (ObservableCollection<object>)vm.TryGetMember(oneWaySeq));
                case VmBinding<TModel, TMsg>.TwoWay twoWay:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(twoWay), (vm, value) => vm.TrySetMember(value, twoWay));
                case VmBinding<TModel, TMsg>.TwoWayValidate twoWayValidate:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(twoWayValidate), (vm, value) => vm.TrySetMember(value, twoWayValidate));
                case VmBinding<TModel, TMsg>.Cmd cmd:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ICommand>(name, vm => vm.TryGetMember(cmd) as ICommand);
                case VmBinding<TModel, TMsg>.CmdParam cmdParam:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name, vm => vm.TryGetMember(cmdParam));
                case VmBinding<TModel, TMsg>.SubModel subModel:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ViewModel<object, object>>(name,
                        vm => vm.TryGetMember(subModel) as ViewModel<object, object>);
                case VmBinding<TModel, TMsg>.SubModelSeq subModelSeq:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ObservableCollection<Uno.ViewModel<object, object>>>(name,
                        vm => (ObservableCollection<Uno.ViewModel<object, object>>)vm.TryGetMember(subModelSeq));
                case VmBinding<TModel, TMsg>.SubModelSelectedItem subModelSelectedItem:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, ViewModel<object, object>>(name,
                        vm => (ViewModel<object, object>) vm.TryGetMember(subModelSelectedItem));
                case VmBinding<TModel, TMsg>.Cached cached:
                    return new DynamicCustomProperty<ViewModel<TModel, TMsg>, object>(name,
                        vm => vm.TryGetMember(cached), (vm, value) => vm.TrySetMember(value, cached));
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

namespace Elmish.Uno
{
    [RequireQualifiedAccess, CompilationMapping(SourceConstructFlags.Module)]
    public static class ViewModel
    {
        public static object DesignInstance<TModel, TMsg>(TModel model, FSharpList<Binding<TModel, TMsg>> bindings)
        {
            var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
            return new Elmish.Windows.ViewModel<TModel, TMsg>(model, emptyDispatch, bindings, ElmConfig.Default, "main");
        }

        public static object DesignInstance<T, TModel, TMsg>(TModel model, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program)
        {
            var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
            var mapping = FSharpFunc<TModel, FSharpFunc<TMsg, Unit>>.InvokeFast(ProgramModule.view(program), model, emptyDispatch);
            return DesignInstance(model, mapping);
        }

        public static void StartLoop<TModel, TMsg>(ElmConfig config, FrameworkElement element, Action<Program<Microsoft.FSharp.Core.Unit, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>>> programRun, Program<Unit, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program)
        {
            FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel = new FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>>(null);
            FSharpFunc<FSharpFunc<TMsg, Unit>, FSharpFunc<TMsg, Unit>> syncDispatch =
              FuncConvert.FromAction(MakeSyncDispatch<TMsg>(element));
            var setSate = FuncConvert.FromAction(MakeSetState(config, element, program, lastModel));
            programRun.Invoke(
                ProgramModule.withSyncDispatch(syncDispatch,
                  ProgramModule.withSetState(setSate, program)));
        }

        public static void StartLoop<T, TModel, TMsg>(ElmConfig config, FrameworkElement element, Action<T, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>>> programRun, Program<T, TModel, TMsg, FSharpList<Binding<TModel, TMsg>>> program, T arg)
        {
            FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>> lastModel = new FSharpRef<FSharpOption<ViewModel<TModel, TMsg>>>(null);
            FSharpFunc<FSharpFunc<TMsg, Unit>, FSharpFunc<TMsg, Unit>> syncDispatch =
              FuncConvert.FromAction(MakeSyncDispatch<TMsg>(element));
            var setSate = FuncConvert.FromAction(MakeSetState(config, element, program, lastModel));

            programRun.Invoke(arg,
                ProgramModule.withSyncDispatch(syncDispatch,
                  ProgramModule.withSetState(setSate, program)));
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
                var viewModel = new Elmish.Windows.ViewModel<TModel, TMsg>(model, dispatch, Bindings, config, "main");
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
