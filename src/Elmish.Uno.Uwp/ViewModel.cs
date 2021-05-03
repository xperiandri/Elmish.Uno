﻿using System;
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
    public class DynamicCustomProperty<TValue> : ICustomProperty
    {
        public Func<TValue> Getter { get; }
        public Action<TValue> Setter { get; }
        public Func<object, TValue> IndexGetter { get; }
        public Action<object, TValue> IndexSetter { get; }

        public object GetValue(object target) => Getter.Invoke();
        public void SetValue(object target, object value) => Setter.Invoke((TValue)value);
        public object GetIndexedValue(object target, object index) => IndexGetter.Invoke(index);
        public void SetIndexedValue(object target, object value, object index) => IndexSetter.Invoke(index, (TValue)value);

        public bool CanRead => Getter != null || IndexGetter != null;
        public bool CanWrite => Setter != null || IndexSetter != null;
        public string Name { get; }
        public Type Type => typeof(TValue);

        public DynamicCustomProperty(string name, Func<TValue> getter, Action<TValue> setter = null, Func<object, TValue> indexGetter = null, Action<object, TValue> indexSetter = null)
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
        public ViewModel(TModel initialModel, FSharpFunc<TMsg, Unit> dispatch, FSharpList<Binding<TModel, TMsg>> bindings, ElmConfig config, string propNameChain) : base(initialModel, dispatch, bindings, config, propNameChain) { }

        public override ViewModelBase<TSubModel, TSubMsg> Create<TSubModel, TSubMsg>(TSubModel initialModel, FSharpFunc<TSubMsg, Unit> dispatch, FSharpList<Binding<TSubModel, TSubMsg>> bindings, ElmConfig config, string propNameChain)
         => new ViewModel<TSubModel, TSubMsg>(initialModel, dispatch, bindings, config, propNameChain);

        public override ObservableCollection<T> CreateCollection<T>(FSharpFunc<TModel, bool> hasMoreItems, FSharpFunc<Tuple<uint, TaskCompletionSource<uint>>, TMsg> loadMoreitems, System.Collections.Generic.IEnumerable<T> collection)
        {
            void LoadMoreitems(uint count, TaskCompletionSource<uint> tcs)
            {
                var msg = loadMoreitems.Invoke(new Tuple<uint, TaskCompletionSource<uint>>(count, tcs));
                Dispatch.Invoke(msg);
            }
            return new IncrementalLoadingCollection<T>(collection, () => hasMoreItems.Invoke(this.currentModel), LoadMoreitems);
        }


        private ICustomProperty GetProperty(string name)
        {
            if (name == "CurrentModel") return new DynamicCustomProperty<object>(name, () => this.CurrentModel);
            if (name == "HasErrors") return new DynamicCustomProperty<bool>(name, () => ((INotifyDataErrorInfo)this).HasErrors);
            if (!this.Bindings.TryGetValue(name, out var binding)) Debugger.Break();
            switch (binding)
            {
                case VmBinding<TModel, TMsg>.OneWay oneWay:
                    return new DynamicCustomProperty<object>(name, () => TryGetMember(oneWay));
                case VmBinding<TModel, TMsg>.OneWayLazy oneWayLazy:
                    return new DynamicCustomProperty<object>(name, () => TryGetMember(oneWayLazy));
                case VmBinding<TModel, TMsg>.OneWaySeq oneWaySeq:
                    return new DynamicCustomProperty<ObservableCollection<object>>(name,
                        () => (ObservableCollection<object>)TryGetMember(oneWaySeq));
                case VmBinding<TModel, TMsg>.TwoWay twoWay:
                    return new DynamicCustomProperty<object>(name, () => TryGetMember(twoWay), value => TrySetMember(value, twoWay));
                case VmBinding<TModel, TMsg>.TwoWayValidate twoWayValidate:
                    return new DynamicCustomProperty<object>(name, () => TryGetMember(twoWayValidate), value => TrySetMember(value, twoWayValidate));
                case VmBinding<TModel, TMsg>.Cmd cmd:
                    return new DynamicCustomProperty<ICommand>(name, () => TryGetMember(cmd) as ICommand);
                case VmBinding<TModel, TMsg>.CmdParam cmdParam:
                    return new DynamicCustomProperty<object>(name, () => TryGetMember(cmdParam));
                case VmBinding<TModel, TMsg>.SubModel subModel:
                    return new DynamicCustomProperty<ViewModel<object, object>>(name,
                        () => TryGetMember(subModel) as ViewModel<object, object>);
                case VmBinding<TModel, TMsg>.SubModelSeq subModelSeq:
                    return new DynamicCustomProperty<ObservableCollection<ViewModelBase<object, object>>>(name,
                        () => (ObservableCollection<ViewModelBase<object, object>>)TryGetMember(subModelSeq));
                case VmBinding<TModel, TMsg>.SubModelSelectedItem subModelSelectedItem:
                    return new DynamicCustomProperty<ViewModel<object, object>>(name,
                        () => (ViewModel<object, object>)TryGetMember(subModelSelectedItem));
                case VmBinding<TModel, TMsg>.Cached cached:
                    return new DynamicCustomProperty<object>(name,
                        () => TryGetMember(cached), value => TrySetMember(value, cached));
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

    public static class ViewModel
    {
        public static object DesignInstance<TModel, TMsg>(TModel model, FSharpList<Binding<TModel, TMsg>> bindings)
        {
            var emptyDispatch = FuncConvert.FromAction((TMsg msg) => { });
            return new ViewModel<TModel, TMsg>(model, emptyDispatch, bindings, ElmConfig.Default, "main");
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
