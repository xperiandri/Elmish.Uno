namespace SolutionTemplate.Programs.App

open System
open Elmish
open Elmish.Uno

open SolutionTemplate.Configuration
open SolutionTemplate.Programs.Messages
open SolutionTemplate.Programs
open SolutionTemplate.Models
open Microsoft.Extensions.Options

type Model = {
    Text: string
    Notifications: Notifications.Model
}

type Msg =
    | ResetSearchText
    | SetSearchText of string
    | NotificationMsg of Notifications.Msg
    | ProcessExistingUserLoggedIn
    | ProcessLoggedOut

type Program
    (notificationProgram : Notifications.Program
    ,graphQLOptions : IOptions<GraphQLOptions>) =

    let init () =
        let notificationModel = notificationProgram.Initial
        { Text = $"Привет от Elmish.Uno. Endpoint '{graphQLOptions.Value.EndPoint}'"
          Notifications = notificationModel },
        Cmd.none

    let update msg m : Model * Cmd<ProgramMessage<RootMsg, Msg>>=
        match msg with
        | Msg.ResetSearchText -> { m with Text = String.Empty }, Cmd.none
        | Msg.SetSearchText text -> { m with Text = text }, Cmd.none
        | Msg.NotificationMsg notificationMsg ->
            let model, cmd = notificationProgram.Update notificationMsg m.Notifications
            { m with Notifications = model }, Cmd.mapLocal NotificationMsg cmd
        | _ -> m, Cmd.none

    let updateGlobal (msg : RootMsg) (m : Model) : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | RootMsg.Notify notification -> m, notification |> Notifications.Cmd.add NotificationMsg

    let updateRoot msg (m : Model) : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | ProgramMessage.Global msg -> updateGlobal msg m
        | ProgramMessage.Local msg -> update msg m

    let bindings = [
        "Text" |> Binding.twoWay ((fun m -> string m.Text), (fun v m -> string v |> SetSearchText |> Local))
        "Notify" |> Binding.cmd (fun m -> Notification.InfoWithTimer "Title" m.Text (TimeSpan.FromSeconds 10.0)
                                          |> Notifications.Msg.AddNotification |> NotificationMsg |> Local)
        "Notifications"
        |> Binding.subModel ((fun m -> m.Notifications), snd, Local << NotificationMsg, notificationProgram.Bindings)
    ]

    [<CompiledName("Program")>]
    member val Program = Program.mkProgramUno init updateRoot bindings
                         |> Program.withConsoleTrace
