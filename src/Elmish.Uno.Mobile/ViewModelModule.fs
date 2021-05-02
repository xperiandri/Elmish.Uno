namespace Elmish.Uno

open System
open Windows.UI.Xaml
open Windows.UI.Core

open Elmish
open Elmish.Uno

type internal ViewModel<'model, 'msg>(initialModel: 'model, dispatch: Dispatch<'msg>, bindings: Binding<'model, 'msg> list, config, propNameChain) =
  inherit ViewModelBase<'model, 'msg>(initialModel, dispatch, bindings, config, propNameChain)

  override _.Create<'subModel,'subMsg>(initialModel: 'subModel, dispatch: Dispatch<'subMsg>, bindings: Binding<'subModel, 'subMsg> list, config: ElmConfig, propNameChain: string) = //raise <| NotImplementedException()
    ViewModel<'subModel,'subMsg>(initialModel, dispatch, bindings, config, propNameChain) :> _

  override this.CreateCollection(hasMoreItems, loadMoreItems: LoadMoreItems<'msg>, collection: 't seq) =
    IncrementalLoadingCollection<'t>(collection, hasMoreItems, loadMoreItems >> this.Dispatch) :> _


[<AbstractClass;Sealed>]
type ViewModel() =
  /// Starts the Elmish dispatch loop, setting the bindings as the DataContext
  /// for the specified FrameworkElement. Non-blocking. This is a low-level function;
  static let startLoop
    (config: ElmConfig)
    (element: FrameworkElement)
    (programRun: Program<'t, 'model, 'msg, Binding<'model, 'msg> list> -> unit)
    (program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>) =

    let mutable lastModel = None

    let setState model dispatch =
      match lastModel with
      | None ->
        let bindings = Program.view program model dispatch
        let vm = ViewModel<'model,'msg>(model, dispatch, bindings, config, "main")
        element.DataContext <- box vm
        lastModel <- Some vm
      | Some vm ->
        vm.UpdateModel model

    let uiDispatch (innerDispatch: Dispatch<'msg>) : Dispatch<'msg> =
      fun msg ->
      let doDispatch = fun () ->
        Console.WriteLine "Dispatch"
        innerDispatch msg
      element.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, fun () -> doDispatch()) |> ignore

    program
    |> Program.withSetState setState
    |> Program.withSyncDispatch uiDispatch
    |> programRun

  static member StartLoop (config, element, programRun : Action<Program<Unit, 'model, 'msg, Binding<'model, 'msg> list>>, program) =
    startLoop config element (FuncConvert.FromAction programRun) program

  static member StartLoop (config, element, programRun : Action<'arg, Program<'arg, 'model, 'msg, Binding<'model, 'msg> list>>, program, arg) =
    startLoop config element (FuncConvert.FromAction programRun arg) program

  static member DesignInstance (model: 'model, bindings: Binding<'model, 'msg> list) =
    ViewModel<_,_> (model, ignore, bindings, ElmConfig.Default, "main") |> box

  static member DesignInstance (model: 'model, program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>) =
    let bindings = Program.view program model ignore
    ViewModel.DesignInstance (model, bindings)

