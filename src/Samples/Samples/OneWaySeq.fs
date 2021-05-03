module Elmish.Uno.Samples.OneWaySeq.Program

open System.Threading.Tasks
open FSharp.Collections.Immutable
open Elmish
open Elmish.Uno


type Model =
  { OneWaySeqNumbers: int list
    OneWayNumbers: int list
    IncrementalLoadingNumbers: int FlatList }

let initial =
  { OneWaySeqNumbers = [ 1000..-1..1 ]
    OneWayNumbers = [ 1000..-1..1 ]
    IncrementalLoadingNumbers = [ 1..1..10 ] |> FlatList.ofSeq }

let init () = initial

type Msg =
  | AddOneWaySeqNumber
  | AddOneWayNumber
  | LoadMore of count: uint * tcs: TaskCompletionSource<uint>

let update msg m =
  match msg with
  | AddOneWaySeqNumber -> { m with OneWaySeqNumbers = m.OneWaySeqNumbers.Head + 1 :: m.OneWaySeqNumbers }
  | AddOneWayNumber -> { m with OneWayNumbers = m.OneWayNumbers.Head + 1 :: m.OneWayNumbers }
  | LoadMore (count, tcs) ->
    let intCount = int count
    let builder = m.IncrementalLoadingNumbers.ToBuilder()
    let max = FlatList.last m.IncrementalLoadingNumbers
    for i = max + 1 to max + intCount do
      builder.Add(i)
    tcs.SetResult(count)
    { m with IncrementalLoadingNumbers = builder.ToImmutable() }

let bindings : Binding<Model, Msg> list = [
  "OneWaySeqNumbers" |> Binding.oneWaySeq ((fun m -> m.OneWaySeqNumbers), (=), id)
  "IncrementalLoadingNumbers" |> Binding.oneWaySeq ((fun m -> m.IncrementalLoadingNumbers), (=), id, (fun _ -> true), LoadMore)
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
