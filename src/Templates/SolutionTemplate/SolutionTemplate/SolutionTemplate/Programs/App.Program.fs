namespace SolutionTemplate.Programs.App

open System
open Elmish
open Elmish.Uno

open SolutionTemplate.Models
open SolutionTemplate.Programs.Messages
open SolutionTemplate.Programs

type Model = {
    Text: string
    Notifications: Notification.Model
}

type Msg =
    | ResetSearchText
    | SetSearchText of string
    | NotificationMsg of Notification.Msg

type Program(notificationProgram: Notification.Program) =

    let init () =
        let notificationModel = notificationProgram.Initial
        { Text = "Привет от Elmish.Uno"
          Notifications = notificationModel },
        Cmd.none

    let update msg m : Model * Cmd<ProgramMessage<RootMsg, Msg>>=
        match msg with
        | Msg.ResetSearchText -> { m with Text = String.Empty }, Cmd.none
        | Msg.SetSearchText text -> { m with Text = text }, Cmd.none
        | Msg.NotificationMsg _ -> m, Cmd.none

    let updateGlobal (msg: RootMsg) (m: Model): Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        let addNotification notification: Cmd<ProgramMessage<RootMsg, Msg>> =
            notification
            |> Notification.Msg.AddNotification
            |> NotificationMsg
            |> Local
            |> Cmd.ofMsg

        match msg with
        | RootMsg.Notify n -> m, addNotification n

    let updateRoot msg (m: Model): Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | ProgramMessage.Global msg -> updateGlobal msg m
        | ProgramMessage.Local msg -> update msg m

    let bindings = [
        "Text" |> Binding.twoWay ((fun m -> string m.Text), (fun v m -> string v |> SetSearchText |> Local))
        "Notifications"
        |> Binding.subModel ((fun m -> m.Notifications), snd, Local << NotificationMsg, notificationProgram.Bindings)
    ]

    [<CompiledName("Program")>]
    member val Program = Program.mkProgramUno init updateRoot bindings
                         |> Program.withConsoleTrace
