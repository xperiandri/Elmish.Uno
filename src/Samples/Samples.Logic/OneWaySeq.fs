module Elmish.Uno.Samples.OneWaySeq.Program

open System
open Elmish
open Elmish.Uno
open Microsoft.Extensions.Logging


type Model =
  { OneWaySeqNumbers: int list
    OneWayNumbers: int list }

type Msg =
  | AddOneWaySeqNumber
  | AddOneWayNumber

let initial =
  { OneWaySeqNumbers = [ 1000..-1..1 ]
    OneWayNumbers = [ 1000..-1..1 ] }

let init () = initial, Cmd.ofMsg AddOneWaySeqNumber

let update msg m =
  match msg with
  | AddOneWaySeqNumber -> { m with OneWaySeqNumbers = m.OneWaySeqNumbers.Head + 1 :: m.OneWaySeqNumbers }, Cmd.none
  | AddOneWayNumber -> { m with OneWayNumbers = m.OneWayNumbers.Head + 1 :: m.OneWayNumbers }, Cmd.none

[<CompiledName "Bindings">]
let bindings : Binding<Model, Msg> list = [
  "OneWaySeqNumbers" |> Binding.oneWaySeq ((fun m -> m.OneWaySeqNumbers), (=), id)
  "OneWayNumbers" |> Binding.oneWay (fun m -> m.OneWayNumbers)
  "AddOneWaySeqNumber" |> Binding.cmd AddOneWaySeqNumber
  "AddOneWayNumber" |> Binding.cmd AddOneWayNumber
]

[<CompiledName("DesignInstance")>]
let designInstance = ViewModel.designInstance initial bindings

[<CompiledName("Program")>]
let program =
  //UnoProgram.mkSimple init update bindings
  UnoProgram.mkProgram init update bindings

type ViewModel() as vm =
  inherit DynamicViewModel<Model, Msg>(
    UnoProgram.createVmArgs (Func<_>(fun () -> vm)) program,
    bindings
  )
