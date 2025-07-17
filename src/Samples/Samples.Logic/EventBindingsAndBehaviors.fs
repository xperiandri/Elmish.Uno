module Elmish.Uno.Samples.EventBindingsAndBehaviors.Program

open Elmish
open Elmish.Uno
open Microsoft.UI.Xaml
open Windows.UI.Core
open Serilog
open Serilog.Extensions.Logging

type Position = { X: int; Y: int }

type Model =
  { Msg1: string
    Msg2: string
    ButtonText: string
    Visibility: Visibility
    CommandParameter: string | null
    MousePosition: Position }

let visibleButtonText = "Hide text box"
let collapsedButonText = "Show text box"

let initial =
  { Msg1 = ""
    Msg2 = ""
    ButtonText = visibleButtonText
    Visibility = Visibility.Visible
    CommandParameter = "parameter"
    MousePosition = { X = 0; Y = 0 } }

let init () = initial

type Msg =
  | GotFocus1
  | GotFocus2
  | LostFocus1
  | LostFocus2
  | ToggleVisibility
  | NewPointerPosition of Position
  | CommandParameterChanged of string
  | SavedCommandParameter

let update msg m =
  match msg with
  | GotFocus1 -> { m with Msg1 = "Focused" }
  | GotFocus2 -> { m with Msg2 = "Focused" }
  | LostFocus1 -> { m with Msg1 = "Not focused" }
  | LostFocus2 -> { m with Msg2 = "Not focused" }
  | ToggleVisibility ->
    if m.Visibility = Visibility.Visible
    then { m with Visibility = Visibility.Collapsed; ButtonText = collapsedButonText }
    else { m with Visibility = Visibility.Visible; ButtonText = visibleButtonText }
  | NewPointerPosition p -> { m with MousePosition = p }
  | CommandParameterChanged p ->
    { m with CommandParameter = p }
  | SavedCommandParameter ->
    match m.CommandParameter with
    | null ->
      Log.Information("Command parameter is null")
      m
    | p ->
      Log.Information("Command parameter saved: {CommandParameter}", p)
      m


let paramToNewMousePositionMsg (p: objnull) =
  let args = p |> nonNull :?> PointerEventArgs
  //let e = args.OriginalSource :?> UIElement;
  let point = args.CurrentPoint.Position
  NewPointerPosition { X = int point.X; Y = int point.Y }

let canSaveCommandParameter (m: Model) =
  m.CommandParameter <> null
let bindings : Binding<Model, Msg> list = [
  "Msg1" |> Binding.oneWay (fun m -> m.Msg1)
  "Msg2" |> Binding.oneWay (fun m -> m.Msg2)
  "GotFocus1" |> Binding.cmd GotFocus1
  "GotFocus2" |> Binding.cmd GotFocus2
  "LostFocus1" |> Binding.cmd LostFocus1
  "LostFocus2" |> Binding.cmd LostFocus2
  "ToggleVisibility" |> Binding.cmd ToggleVisibility
  "ButtonText" |> Binding.oneWay (fun m -> m.ButtonText)
  "TextBoxVisibility" |> Binding.oneWay (fun m -> m.Visibility)
  "MouseMoveCommand" |> Binding.cmdParam paramToNewMousePositionMsg
  "MousePosition" |> Binding.oneWay (fun m -> sprintf "%dx%d" m.MousePosition.X m.MousePosition.Y)
  "CommandParameter" |> Binding.cmdParamIf ((fun _ _ -> SavedCommandParameter), (fun p m -> canSaveCommandParameter m))
]

[<CompiledName("DesignInstance")>]
let designInstance = ViewModel.designInstance initial bindings

[<CompiledName("Program")>]
let program =
    UnoProgram.mkSimple init update bindings
    |> UnoProgram.withLogger (new SerilogLoggerFactory(logger))
