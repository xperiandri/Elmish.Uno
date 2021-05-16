module Elmish.Uno.Samples.Validation.Program

open System
open System.Linq
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
  >> Result.mapError box


let validatePassword (s: string) =
  [
    if s.All(fun c -> Char.IsDigit c |> not) then
      "Must contain a digit" |> box
    if s.All(fun c -> Char.IsLower c |> not) then
      "Must contain a lowercase letter" |> box
    if s.All(fun c -> Char.IsUpper c |> not) then
      "Must contain an uppercase letter" |> box
  ]


type Model =
  { Value: string
    Password: string }

let initial =
  { Value = ""
    Password = "" }

let init () = initial

type Msg =
  | NewValue of string
  | NewPassword of string
  | Submit

let update msg m =
  match msg with
  | NewValue x -> { m with Value = x }
  | NewPassword x -> { m with Password = x }
  | Submit -> m

let bindings : Binding<Model, Msg> list = [
  "Value" |> Binding.twoWayValidate(
    (fun m -> m.Value),
    NewValue,
    (fun m ->  validateInt42 m.Value),
    id)
  "Password" |> Binding.twoWayValidate(
    (fun m -> m.Password),
    NewPassword,
    (fun m -> validatePassword m.Password),
    id)
  "Submit" |> Binding.cmdIf(
    (fun _ -> Submit),
    (fun m -> (match validateInt42 m.Value with Ok _ -> true | Error _ -> false) && (validatePassword m.Password |> List.isEmpty)))
]

[<CompiledName("DesignModel")>]
let designModel = initial

[<CompiledName("Program")>]
let program =
  Program.mkSimpleUno init update bindings
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true; Measure = true }
