namespace SolutionTemplate.Programs.Notification


open System

open Elmish
open Elmish.Uno
open Elmish.Uno.Navigation
open SolutionTemplate
open SolutionTemplate.Pages
open SolutionTemplate.Models
open SolutionTemplate.Programs.Messages

type Model =
    { Notifications: Notification list }
    static member Initial = { Notifications = [] }

type Msg =
    | AddNotification of Notification
    | RemoveNotification of Notification
    | NavigationalError of String
    | SetCloseNotification of Notification
    | TimerTick
    interface IAppMsg

type public Program() =

    let closeNotification notification (time: int) (dispatch: Dispatch<ProgramMessage<RootMsg, Msg>>) =
        ignore
        <| Async.RunSynchronously(
            Async.StartChild
            <| async {
                do! Async.Sleep time
                do dispatch (Local <| RemoveNotification notification)
               }
        )

    let asyncSetNotificationTimer notification =
        async { return (SetCloseNotification notification) |> Local }

    member p.Initial = Model.Initial

    member p.Update msg (m: Model): Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | AddNotification notification ->
            match m.Notifications
                  |> Seq.tryFind (fun n -> n.Text = notification.Text) with
            | Some _ ->
                let n =
                    m.Notifications
                    |> Seq.map
                        (fun n ->
                            if n.Text = notification.Text then
                                { Text = notification.Text
                                  Type = notification.Type
                                  Title = notification.Title
                                  LifeTime = notification.LifeTime
                                  IsOpen = true
                                  IsClosable = notification.IsClosable }
                            else
                                n)

                { m with
                      Notifications = n |> Seq.toList },
                Cmd.none
            | None ->
                let n = notification :: m.Notifications

                { m with
                      Notifications = n |> Seq.toList },
                Cmd.OfAsync.result (asyncSetNotificationTimer (notification))
        | SetCloseNotification notification ->
            match notification.LifeTime with
            | ValueSome timeout ->
                m,
                (closeNotification notification timeout)
                |> Cmd.ofSub
            | ValueNone -> m, Cmd.none
        | RemoveNotification notification ->
            let n =
                m.Notifications
                |> List.filter (fun n -> n.Text <> notification.Text)

            { m with Notifications = n }, Cmd.none

    member p.Bindings(): Binding<Model, Msg> list =
        [ "NavigationalErrorCommand"
          |> Binding.cmdParam (fun o m -> NavigationalError(o :?> string))
          "RemoveNotificationCommand"
          |> Binding.cmdParam (fun o m -> RemoveNotification(o :?> Notification))

          "Notifications"
          |> Binding.oneWaySeq ((fun m -> m.Notifications |> Seq.rev), (=), (fun i -> i.Text)) ]
