module Elmish.Uno.Samples.UiBoundCmdParam.Program

open Elmish
open Elmish.Uno
open Serilog
open Serilog.Extensions.Logging


type Model =
  { Numbers: int list
    EnabledMaxLimit: int }

let init () =
  { Numbers = [0 .. 10]
    EnabledMaxLimit = 5 }

type Msg =
  | SetLimit of int
  | Command

let update msg m =
  match msg with
  | SetLimit x -> { m with EnabledMaxLimit = x }
  | Command -> m

let bindings () : Binding<Model, Msg> list = [
  "Numbers" |> Binding.oneWay(fun m -> m.Numbers)
  "Limit" |> Binding.twoWay((fun m -> float m.EnabledMaxLimit), int >> SetLimit)
  "Command" |> Binding.cmdParamIf(
    (fun p m -> Command),
    (fun p m -> not (isNull p) && p :?> int <= m.EnabledMaxLimit),
    true)
]

let designVm = ViewModel.designInstance (init ()) (bindings ())

let main window =
  let logger =
    LoggerConfiguration()
      .MinimumLevel.Override("Elmish.Uno.Update", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.Uno.Bindings", Events.LogEventLevel.Verbose)
      .MinimumLevel.Override("Elmish.Uno.Performance", Events.LogEventLevel.Verbose)
      .WriteTo.Console()
      .CreateLogger()

  Program.mkSimple init update bindings
  |> Program.withLogger (new SerilogLoggerFactory(logger))
  |> Program.startElmishLoop window
