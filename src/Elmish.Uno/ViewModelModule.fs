namespace Elmish.Uno

open System
open Windows.UI.Core
open Microsoft.UI.Xaml

open Elmish
open Elmish.Uno

[<AbstractClass;Sealed>]
type ViewModel() =
  /// Starts the Elmish dispatch loop, setting the bindings as the DataContext
  /// for the specified FrameworkElement. Non-blocking. This is a low-level function;
  static let startLoop
    (config: ElmConfig)
    (element: FrameworkElement)
    (program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>)
    arg
    =

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
      element.DispatcherQueue.TryEnqueue(fun () -> doDispatch()) |> ignore

    program
    |> Program.withSetState setState
    |> Program.runWithDispatch uiDispatch arg

  static member StartLoop (config, element, program) =
    startLoop config element program ()

  static member StartLoop (config, element, program, arg) =
    startLoop config element program arg

  static member DesignInstance (model: 'model, bindings: Binding<'model, 'msg> list) =
    ViewModel<_,_> (model, ignore, bindings, ElmConfig.Default, "main") |> box

  static member DesignInstance (model: 'model, program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>) =
    let bindings = Program.view program model ignore
    ViewModel.DesignInstance (model, bindings)

