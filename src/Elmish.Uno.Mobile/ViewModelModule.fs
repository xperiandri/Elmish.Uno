[<RequireQualifiedAccess>]
module Elmish.Uno.ViewModel

open System
open Windows.UI.Xaml
open Windows.UI.Core

open Elmish
open Elmish.Uno

/// Starts the Elmish dispatch loop, setting the bindings as the DataContext
/// for the specified FrameworkElement. Non-blocking. This is a low-level function;
let startLoop
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

let StartLoop (config, element, programRun : Action<Program<'t, 'model, 'msg, Binding<'model, 'msg> list>>, program) =
    startLoop config element (FuncConvert.FromAction programRun) program


/// Creates a design-time view model using the given model and bindings.
let designInstance (model: 'model) (program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>) =
    let bindings = Program.view program model ignore
    ViewModel (model, ignore, bindings, ElmConfig.Default, "main") |> box

let DesignInstance (model: 'model, program: Program<'t, 'model, 'msg, Binding<'model, 'msg> list>) =
    designInstance model program
