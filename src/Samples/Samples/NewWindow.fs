module Elmish.Uno.Samples.NewWindow.Program

open System
open System.Windows
open Elmish
open Elmish.Uno
open Windows.ApplicationModel.Core
open Windows.UI.Core
open Windows.UI.ViewManagement
open Microsoft.UI.Xaml
open Microsoft.UI.Xaml.Controls

module Win1 =

  type Model = { Text: string }

  type Msg =
  | TextInput of string

  let init = { Text = "" }

  let update msg m =
    match msg with
    | TextInput s -> { m with Text = s }

  [<CompiledName("Bindings")>]
  let bindings () =
    [ "Text" |> Binding.twoWay ((fun m -> m.Text), (fun v m -> TextInput v)) ]

  [<CompiledName("DesignModel")>]
  let designModel = init


module Win2 =

  type Model =
    { Input1: string
      Input2: string }

  type Msg =
  | Text1Input of string
  | Text2Input of string

  let init =
    { Input1 = ""
      Input2 = "" }

  let update msg m =
    match msg with
    | Text1Input s -> { m with Input1 = s }
    | Text2Input s -> { m with Input2 = s }

  [<CompiledName("Bindings")>]
  let bindings () = [
    "Input1" |> Binding.twoWay ((fun m -> m.Input1), (fun v m -> Text1Input v))
    "Input2" |> Binding.twoWay ((fun m -> m.Input2), (fun v m -> Text2Input v))
  ]

  [<CompiledName("DesignModel")>]
  let designModel = init


type Model =
  { Win1: Win1.Model
    Win2: Win2.Model }

let initial =
  { Win1 = Win1.init
    Win2 = Win2.init }

let init () = initial, Cmd.none

type Msg =
| ShowWin1
| ShowWin2
| Win1Msg of Win1.Msg
| Win2Msg of Win2.Msg

let showWindow windowTitle pageType viewModel = async {
  let view = CoreApplication.CreateNewView()
  do! view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, fun () ->
      let window = CoreWindow.GetForCurrentThread ()
      let view = ApplicationView.GetForCurrentView ()
      view.Title <- windowTitle

      let frame = new Frame()
      frame.DataContext <- viewModel
      frame.Navigate(pageType) |> ignore
      Window.Current.Content <- frame
      Window.Current.Activate()
      ).AsTask()
}

let update window1PageType window2pageType getViewModel msg m =
  match msg with
  | ShowWin1 -> m, Cmd.OfAsync.attempt (showWindow "Window 1" window1PageType) (getViewModel ()) raise
  | ShowWin2 -> m, Cmd.OfAsync.attempt (showWindow "Window 2" window2pageType) (getViewModel ()) raise
  | Win1Msg msg' -> { m with Win1 = Win1.update msg' m.Win1 }, Cmd.none
  | Win2Msg msg' -> { m with Win2 = Win2.update msg' m.Win2 }, Cmd.none

[<CompiledName("Bindings")>]
let bindings = [
  "ShowWin1" |> Binding.cmd (fun m -> ShowWin1)
  "ShowWin2" |> Binding.cmd (fun m -> ShowWin2)
  "Win1" |> Binding.subModel ((fun m -> m.Win1), snd, Win1Msg, Win1.bindings)
  "Win2" |> Binding.subModel ((fun m -> m.Win2), snd, Win2Msg, Win2.bindings)
]

[<CompiledName("DesignModel")>]
let designModel = initial

[<CompiledName("CreateProgram")>]
let createProgram<'win1, 'win2> getViewModel =
    Program.mkProgramUno init (update typeof<'win1> typeof<'win2> getViewModel) bindings
    |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
