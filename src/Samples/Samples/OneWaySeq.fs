module Elmish.Uno.Samples.OneWaySeq.Program

open System
open Elmish
open Elmish.Uno


type Model =
  { OneWaySeqNumbers: int list
    OneWayNumbers: int list }

let initial =
  { OneWaySeqNumbers = [ 1000..-1..1 ]
    OneWayNumbers = [ 1000..-1..1 ] }

let init () = initial

type Msg =
  | AddOneWaySeqNumber
  | AddOneWayNumber

let update msg m =
  match msg with
  | AddOneWaySeqNumber -> { m with OneWaySeqNumbers = m.OneWaySeqNumbers.Head + 1 :: m.OneWaySeqNumbers }
  | AddOneWayNumber -> { m with OneWayNumbers = m.OneWayNumbers.Head + 1 :: m.OneWayNumbers }

let bindings : Binding<Model, Msg> list = [
  "OneWaySeqNumbers" |> Binding.oneWaySeq ((fun m -> m.OneWaySeqNumbers), (=), id)
  "OneWayNumbers" |> Binding.oneWay (fun m -> m.OneWayNumbers)
  "AddOneWaySeqNumber" |> Binding.cmd AddOneWaySeqNumber
  "AddOneWayNumber" |> Binding.cmd AddOneWayNumber
]

[<CompiledName("DesignModel")>]
let designModel = initial

[<CompiledName("Program")>]
let program =
  Program.mkSimpleUno init update bindings
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
