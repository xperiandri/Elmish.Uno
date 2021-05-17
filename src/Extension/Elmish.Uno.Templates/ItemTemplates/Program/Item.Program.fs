namespace $rootnamespace$.Programs.$safeitemname$


open Elmish
open Elmish.Uno
open $rootnamespace$.Pages
open $rootnamespace$.Models
open $rootnamespace$.Programs.Messages

type Model = {
    Text : string
}

type Msg =
    | SetText of string

type public Program () =

   let initial () =
       { Text = "Hello from Elmish.Uno" }, Cmd.none

   let update msg m : Model * Cmd<ProgramMessage<RootMsg, Msg>>=
       match msg with
       | Msg.SetText text -> { m with Text = text }, Cmd.none

   member p.bindings =
       [
           "Text" |> Binding.twoWay ((fun m -> string m.Text), (fun v m -> string v |> SetText |> Local)) ]
