module Elmish.Uno.Samples.OneWaySeq.Program

open System.Threading.Tasks
open FSharp.Collections.Immutable
open Elmish
open Elmish.Uno
open System


type Model =
  { OneWaySeqNumbers: int list
    OneWayNumbers: int list
    IncrementalLoadingNumbers: int FlatList
    IsLoading: bool }

let initial =
  { OneWaySeqNumbers = [ 1000..-1..1 ]
    OneWayNumbers = [ 1000..-1..1 ]
    IncrementalLoadingNumbers = [ 1..1..10 ] |> FlatList.ofSeq
    IsLoading = false }

let init () = initial, Cmd.none

type Msg =
  | AddOneWaySeqNumber
  | AddOneWayNumber
  | LoadMore of count: uint * complete: (uint -> unit)
  | LoadedMore of allItems: FlatList<int>
  | ErrorLoadingMore

let asyncLoadItems (complete: uint -> unit) (items: FlatList<int>) count = async {
  try
    try
      let intCount = int count
      let builder = items.ToBuilder()
      let max = FlatList.last items
      for i = max + 1 to max + intCount do
        builder.Add(i)
      return LoadedMore <| builder.ToImmutable ()
    with _ ->
      return ErrorLoadingMore
  finally
    // Must be called to complete Task and unblock UI to update
    // Otherwise it will block loading async operaiton and it will never be called again
    complete count
 }

let update msg m =
  match msg with
  | AddOneWaySeqNumber -> { m with OneWaySeqNumbers = m.OneWaySeqNumbers.Head + 1 :: m.OneWaySeqNumbers }, Cmd.none
  | AddOneWayNumber -> { m with OneWayNumbers = m.OneWayNumbers.Head + 1 :: m.OneWayNumbers }, Cmd.none
  | LoadMore (count, complete) ->
                        // If error hapened earlier it may be worth to do someting with that instead
                        { m with IsLoading = true }, // Display some hint (indetermined progress bar)
                        asyncLoadItems complete m.IncrementalLoadingNumbers count |> Cmd.OfAsync.result
  | LoadedMore items -> { m with
                            IsLoading = false // Hide loading indicator
                            IncrementalLoadingNumbers = items }, Cmd.none
  | ErrorLoadingMore -> m, Cmd.none // add error to model that will appear on IU

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
  Program.mkProgramUno init update bindings
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
