module Elmish.Uno.Samples.Validation.Program

open System
open Elmish
open Elmish.Uno

let requireNotEmpty s =
  if String.IsNullOrEmpty s then Error "This field is required" else Ok s

let parseInt (s: string) =
  match Int32.TryParse s with
  | true, i -> Ok i
  | false, _ -> Error "Please enter a valid integer"

let requireExactly y x =
  if x = y then Ok x else Error <| sprintf "Please enter %A" y

let validateInt42 =
  requireNotEmpty
  >> Result.bind parseInt
  >> Result.bind (requireExactly 42)


type Model =
  { RawValue: string }

let initial =
  { RawValue = "" }

let init () = initial

type Msg =
  | Input of string
  | Submit of int

let update msg m =
  match msg with
  | Input x -> { m with RawValue = x }
  | Submit _ -> m

let bindings : Binding<Model, Msg> list = [
  "RawValue" |> Binding.twoWayValidate(
    (fun m -> m.RawValue),
    Input,
    (fun m ->  validateInt42 m.RawValue))
  "Submit" |> Binding.cmdIf(
    fun m -> validateInt42 m.RawValue |> Result.map Submit)
]

[<CompiledName("DesignModel")>]
let designModel = initial

[<CompiledName("Program")>]
let program =
  Program.mkSimpleUno init update bindings
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
