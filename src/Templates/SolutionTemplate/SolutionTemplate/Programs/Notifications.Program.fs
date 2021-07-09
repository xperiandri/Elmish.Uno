namespace SolutionTemplate.Programs.Notifications


open Elmish
open Elmish.Uno
open SolutionTemplate.Pages
open SolutionTemplate.Models
open SolutionTemplate.Programs.Messages
open System

type Model =
    { Notifications: Notification list }
    static member Initial = { Notifications = [] }

type Msg =
    | AddNotification of Notification
    | RemoveNotification of Notification
    | NavigationFailed of NavigationError
    | ScheduleCloseNotification of Notification

module Cmd =
    let add appMsg notification : Cmd<ProgramMessage<RootMsg, 'appMsg>> =
        notification |> Cmd.ofLocal Msg.AddNotification appMsg

type public Program () =

    let closeNotification notification (time : TimeSpan) (dispatch : Dispatch<ProgramMessage<RootMsg, Msg>>) =
        async {
            do! Async.Sleep time
            do dispatch (Local <| RemoveNotification notification)
        } |> Async.StartChild |> Async.RunSynchronously |> ignore

    member p.Initial = Model.Initial

    member p.Update msg (m : Model) : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | AddNotification notification ->
            match m.Notifications
                  |> Seq.tryFind (fun n -> n.Text = notification.Text) with
            | Some _ ->
                let n =
                    m.Notifications
                    |> List.map
                        (fun n ->
                            if n.Text = notification.Text then
                                { Text = notification.Text
                                  Type = notification.Type
                                  Title = notification.Title
                                  Timeout = notification.Timeout
                                  IsOpen = true
                                  IsClosable = notification.IsClosable }
                            else
                                n)
                { m with Notifications = n },
                Cmd.none
            | None ->
                let n = notification :: m.Notifications
                { m with Notifications = n },
                ScheduleCloseNotification notification |> Local |> Cmd.ofMsg
        | ScheduleCloseNotification notification ->
            match notification.Timeout with
            | ValueSome timeout ->
                m,
                (closeNotification notification timeout) |> Cmd.ofSub
            | ValueNone -> m, Cmd.none
        | RemoveNotification notification ->
            let n =
                m.Notifications
                |> List.filter (fun n -> n.Text <> notification.Text)
            { m with Notifications = n }, Cmd.none
        | NavigationFailed error ->
            let n = Notification.Error $"Failed to load {error.SourcePageType.FullName}" (error.Exception.ToString ())
            m, AddNotification n |> Local |> Cmd.ofMsg

    member p.Bindings(): Binding<Model, Msg> list =
        [ "NavigationFailedCommand"
          |> Binding.cmdParam (fun o _ -> NavigationFailed (o :?> NavigationError))
          "RemoveNotificationCommand"
          |> Binding.cmdParam (fun o _ -> RemoveNotification (o :?> Notification))

          "Notifications"
          |> Binding.oneWaySeq ((fun m -> m.Notifications |> Seq.rev), (=), (fun i -> i.Text)) ]
