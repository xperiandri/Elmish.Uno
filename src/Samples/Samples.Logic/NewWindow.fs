module Elmish.Uno.Samples.NewWindow.Program

open System
open Elmish
open Elmish.Uno
open Microsoft.UI.Xaml
open Microsoft.UI.Xaml.Controls

module Win1 =

  type Model = { Text: string }

  type Msg =
  | TextInput of string

  let initial = { Text = "" }

  let update msg m =
    match msg with
    | TextInput s -> { m with Text = s }

  [<CompiledName("Bindings")>]
  let bindings =
    [ "Text" |> Binding.twoWay ((fun m -> m.Text), TextInput) ]

  [<CompiledName("DesignInstance")>]
  let designInstance = ViewModel.designInstance initial bindings


module Win2 =

  type Model =
    { Input1: string
      Input2: string }

  type Msg =
  | Text1Input of string
  | Text2Input of string

  let initial =
    { Input1 = ""
      Input2 = "" }

  let update msg m =
    match msg with
    | Text1Input s -> { m with Input1 = s }
    | Text2Input s -> { m with Input2 = s }

  [<CompiledName("Bindings")>]
  let bindings = [
    "Input1" |> Binding.twoWay ((fun m -> m.Input1), Text1Input)
    "Input2" |> Binding.twoWay ((fun m -> m.Input2), Text2Input)
  ]

  [<CompiledName("DesignInstance")>]
  let designInstance = ViewModel.designInstance initial bindings


type Model =
  { Win1: Win1.Model
    Win2: Win2.Model }

let initial =
  { Win1 = Win1.initial
    Win2 = Win2.initial }

let init () = initial, Cmd.none

type Msg =
| ShowWin1
| ShowWin2
| Win1Msg of Win1.Msg
| Win2Msg of Win2.Msg

let showWindow windowTitle pageType viewModel =
    let window = new Window()
    window.Title <- windowTitle

    let frame = new Frame()
    frame.DataContext <- viewModel
    frame.Navigate pageType |> ignore
    window.Content <- frame
    window.Activate()

let update window1PageType window2pageType getViewModel msg m =
  match msg with
  | ShowWin1 -> m, Cmd.OfFunc.attempt (showWindow "Window 1" window1PageType) (getViewModel ()) raise
  | ShowWin2 -> m, Cmd.OfFunc.attempt (showWindow "Window 2" window2pageType) (getViewModel ()) raise
  | Win1Msg msg' -> { m with Win1 = Win1.update msg' m.Win1 }, Cmd.none
  | Win2Msg msg' -> { m with Win2 = Win2.update msg' m.Win2 }, Cmd.none

[<CompiledName("Bindings")>]
let bindings = [
  "ShowWin1" |> Binding.cmd (fun m -> ShowWin1)
  "ShowWin2" |> Binding.cmd (fun m -> ShowWin2)
  "Win1" |> Binding.subModel ((fun m -> m.Win1), snd, Win1Msg, Win1.bindings)
  "Win2" |> Binding.subModel ((fun m -> m.Win2), snd, Win2Msg, Win2.bindings)
]

[<CompiledName("DesignInstance")>]
let designInstance = ViewModel.designInstance initial bindings

[<CompiledName("CreateProgram")>]
let createProgram<'win1, 'win2> getViewModel =
  UnoProgram.mkProgram init (update typeof<'win1> typeof<'win2> getViewModel) bindings
