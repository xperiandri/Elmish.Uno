module Elmish.Uno.Samples.FileDialogsCmdMsg.Program

open System
open Elmish
open Elmish.Uno


module Core =

  type Model =
    { CurrentTime: DateTimeOffset
      Text: string
      StatusMsg: string }

  type CmdMsg =
    | Save of string
    | Load


  let initial =
    { CurrentTime = DateTimeOffset.Now
      Text = ""
      StatusMsg = "" }

  let init () = initial, []

  type Msg =
    | SetTime of DateTimeOffset
    | SetText of string
    | RequestSave
    | RequestLoad
    | SaveSuccess
    | LoadSuccess of string
    | SaveCanceled
    | LoadCanceled
    | SaveFailed of exn
    | LoadFailed of exn

  let update msg m =
    match msg with
    | SetTime t -> { m with CurrentTime = t }, []
    | SetText s -> { m with Text = s}, []
    | RequestSave -> m, [Save m.Text]
    | RequestLoad -> m, [Load]
    | SaveSuccess -> { m with StatusMsg = sprintf "Successfully saved at %O" DateTimeOffset.Now }, []
    | LoadSuccess s -> { m with Text = s; StatusMsg = sprintf "Successfully loaded at %O" DateTimeOffset.Now }, []
    | SaveCanceled -> { m with StatusMsg = "Saving canceled" }, []
    | LoadCanceled -> { m with StatusMsg = "Loading canceled" }, []
    | SaveFailed ex -> { m with StatusMsg = sprintf "Saving failed with excption %s: %s" (ex.GetType().Name) ex.Message }, []
    | LoadFailed ex -> { m with StatusMsg = sprintf "Loading failed with excption %s: %s" (ex.GetType().Name) ex.Message }, []


module Platform =

  open System.IO
  open System.Threading
  open System.Windows
  open Core

  let bindings : Binding<Model, Msg> list = [
    "CurrentTime" |> Binding.oneWay (fun m -> m.CurrentTime)
    "Text" |> Binding.twoWay ((fun m -> m.Text), SetText)
    "StatusMsg" |> Binding.twoWay ((fun m -> m.StatusMsg), SetText)
    "Save" |> Binding.cmd RequestSave
    "Load" |> Binding.cmd RequestLoad
  ]

  let save text =
    //CoreApplication.GetCurrentView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, fun () ->
    //  let guiCtx = SynchronizationContext.Current
      async {
        //do! Async.SwitchToContext guiCtx
        let picker = new Windows.Storage.Pickers.FileSavePicker()
        let fileTypeChoices = picker.FileTypeChoices
        do fileTypeChoices.Add("Plain Text", [|".txt"|])
        do fileTypeChoices.Add("Markdown"  , [|".md" |])
        let! file = picker.PickSaveFileAsync().AsTask()
        match file with
        | null -> return SaveCanceled
        | _ ->
          // Prevent updates to the remote version of the file until
          // we finish making changes and call CompleteUpdatesAsync.
          Windows.Storage.CachedFileManager.DeferUpdates(file)
          // write to file
          do! Windows.Storage.FileIO.WriteTextAsync(file, text).AsTask()
          // Let Windows know that we're finished changing the file so
          // the other app can update the remote version of the file.
          // Completing updates may require Windows to ask for user input.
          let! status = Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file).AsTask();
          if status = Windows.Storage.Provider.FileUpdateStatus.Complete
          then return SaveSuccess
          else return SaveFailed (Exception(status.ToString()))
      }
    //).AsTask().AsAsync()

  let load () =
    //CoreApplication.GetCurrentView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, fun () ->
      //let guiCtx = SynchronizationContext.Current
      async {
        //do! Async.SwitchToContext guiCtx
        let picker = new Windows.Storage.Pickers.FileOpenPicker()
        let fileTypeFilter = picker.FileTypeFilter
        do fileTypeFilter.Add(".txt")
        do fileTypeFilter.Add(".md")
        let! file = picker.PickSingleFileAsync().AsTask()
        match file with
        | null -> return LoadCanceled
        | _ ->
          let! contents = Windows.Storage.FileIO.ReadTextAsync(file).AsTask()
          return LoadSuccess contents
      }
    //).AsTask().AsAsync()

  let toCmd = function
    | Save text -> Cmd.OfAsync.either save text id SaveFailed
    | Load -> Cmd.OfAsync.either load () id LoadFailed


open Core
open Platform

let timerTick dispatch =
  let timer = new Timers.Timer(1000.)
  timer.Elapsed.Add (fun _ -> dispatch (SetTime DateTimeOffset.Now))
  timer.Start()


[<CompiledName("DesignModel")>]
let designModel = initial

[<CompiledName("Program")>]
let program =
  Program.mkProgramUnoWithCmdMsg init update bindings toCmd
  |> Program.withSubscription (fun _ -> Cmd.ofSub timerTick)
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
