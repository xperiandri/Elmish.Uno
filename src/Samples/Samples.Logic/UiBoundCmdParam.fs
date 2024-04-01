module Elmish.Uno.Samples.UiBoundCmdParam.Program

open System
open Elmish
open Elmish.Uno

type Model =
  { Numbers: int list
    EnabledMaxLimit: int }

let initial =
  { Numbers = [0 .. 10]
    EnabledMaxLimit = 5 }

let init () = initial

type Msg =
  | SetLimit of int
  | Command

let update msg m =
  match msg with
  | SetLimit x -> { m with EnabledMaxLimit = x }
  | Command -> m

let bindings : Binding<Model, Msg> list = [
  "Numbers" |> Binding.oneWay(fun m -> m.Numbers)
  "Limit" |> Binding.twoWay((fun m -> float m.EnabledMaxLimit), int >> SetLimit)
  "Command" |> Binding.cmdParamIf(
    (fun p m -> Command),
    (fun (p : obj) m -> not (isNull p) && p :?> int <= m.EnabledMaxLimit))
]

[<CompiledName("DesignInstance")>]
let designInstance = ViewModel.designInstance initial bindings

[<CompiledName("Program")>]
let program =
  UnoProgram.mkSimple init update bindings
