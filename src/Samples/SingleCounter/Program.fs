﻿module Elmish.Uno.Samples.SingleCounter.Program

open System
open Elmish
open Elmish.Uno

type Model =
    { Count: int
      StepSize: int }

let init () =
    { Count = 0
      StepSize = 1 }

type Msg =
| Increment
| Decrement
| SetStepSize of int
| Reset

let update msg m =
    match msg with
    | Increment -> { m with Count = m.Count + m.StepSize }
    | Decrement -> { m with Count = m.Count - m.StepSize }
    | SetStepSize x -> { m with StepSize = x }
    | Reset -> init ()

let bindings model dispatch = [
    "CounterValue" |> Binding.oneWay (fun m -> m.Count)
    "Increment" |> Binding.cmd (fun m -> Increment)
    "Decrement" |> Binding.cmd (fun m -> Decrement)
    "StepSize" |> Binding.twoWay (fun m -> float m.StepSize)
                                 (fun v m -> int v |> SetStepSize)
    "Reset" |> Binding.cmdIf (fun _ -> Reset) (fun m -> m <> init ())
]

[<CompiledName("Program")>]
let program =
    Program.mkSimple init update bindings
    |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
