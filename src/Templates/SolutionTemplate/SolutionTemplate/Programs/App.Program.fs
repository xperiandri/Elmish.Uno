namespace SolutionTemplate.Programs.App

open System
open System.Diagnostics
open Elmish
open Elmish.Uno
open Elmish.Uno.Navigation
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Options

open SolutionTemplate
open SolutionTemplate.Elmish
open SolutionTemplate.Options
open SolutionTemplate.Models
open SolutionTemplate.Programs
open SolutionTemplate.Programs.Messages
open SolutionTemplate.WinRT

type Model = {
    Text : string
    Notifications : Notifications.Model
}

type Msg =
    | NetworkChanged of NetworkConnectivityLevel
    | Suspend of deadline : DateTimeOffset * complete : Action
    | Resuming
    | EnteredBackground of complete : Action
    | LeavingBackground of complete : Action
    | UnhandledException of ex : exn * message : string
    | ResetScope
    | OpenPage of Page : Pages
    | ResetSearchText
    | SetSearchText of string
    | NotificationMsg of Notifications.Msg
    | ProcessExistingUserLoggedIn
    | ProcessLoggedOut

type Program (serviceProvider : IServiceProvider) =

    let navigationService = serviceProvider.GetRequiredService<INavigationService>()
    let graphQLOptions = serviceProvider.GetRequiredService<IOptions<GraphQLOptions>>()

    let mutable scope = serviceProvider.CreateScope ()

    let createScope (m : Model) =
        scope <- serviceProvider.CreateScope ()
        m, Cmd.none

    let resetScope (m : Model) =
        let scope' = scope
        let result = createScope m
        scope'.Dispose ()
        result


    let init (previousState : ApplicationExecutionState) : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        let notificationModel = serviceProvider.GetRequiredService<Notifications.Program>().Initial
        let m =
            { Text = $"Привет от Elmish.Uno. State='{previousState}'. GraphQL URL='{graphQLOptions.Value.EndPoint}'"
              Notifications = notificationModel }
        createScope m

    let update msg m : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | UnhandledException (ex, msg) -> Debug.Fail msg; m, Cmd.none
        // TODO: Handle connection changes
        | Msg.NetworkChanged connectivity -> m, Cmd.none
        // TODO: Handle app state changes
        | Msg.Suspend (_, complete) -> complete.Invoke (); m, Cmd.none
        | Msg.Resuming -> m, Cmd.none
        | Msg.EnteredBackground complete -> complete.Invoke (); m, Cmd.none
        | Msg.LeavingBackground complete -> complete.Invoke (); m, Cmd.none
        | Msg.OpenPage page ->
            navigationService.Navigate(page.ToString ()) |> Debug.Assert;
            m, Cmd.none
        | Msg.ResetScope -> resetScope m
        | Msg.ResetSearchText -> { m with Text = String.Empty }, Cmd.none
        | Msg.SetSearchText text -> { m with Text = text }, Cmd.none
        | Msg.NotificationMsg notificationMsg ->
            let notificationProgram = serviceProvider.GetRequiredService<Notifications.Program>()
            let model', cmd = notificationProgram.Update notificationMsg m.Notifications
            { m with Notifications = model' }, cmd |> Cmd.mapLocal NotificationMsg
        | _ ->
            Debugger.Break ()
            m, Cmd.none

    let logOut (dispatch : Dispatch<ProgramMessage<RootMsg, Msg>>) =
        //dispatch <| (Login.LogOut |> LoginMsg |> Local)
        while navigationService.CanGoBack do navigationService.GoBack ()
        dispatch <| (ResetScope |> Local)
        //dispatch <| (Navigate Pages.Login |> Global)

    let updateGlobal (msg: RootMsg) (m: Model): Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        let addNotification notification: Cmd<ProgramMessage<RootMsg, Msg>> =
            notification
            |> Notifications.Msg.AddNotification
            |> NotificationMsg
            |> Local
            |> Cmd.ofMsg

        match msg with
        | RootMsg.LogOut -> m, Cmd.ofSub logOut
        | RootMsg.Notify n -> m, addNotification n
        | RootMsg.Navigate page -> m , OpenPage page |> Local |> Cmd.ofMsg

    let updateRoot msg (m : Model) : Model * Cmd<ProgramMessage<RootMsg, Msg>> =
        match msg with
        | ProgramMessage.Global msg -> updateGlobal msg m
        | ProgramMessage.Local msg -> update msg m

    static let bindings = [
        // Throttling is used to prevent focus reset on TextBox
        "Text" |> Binding.twoWay ((fun m -> string m.Text), (fun v m -> string v |> SetSearchText |> Local), throttle)
        "NotifyCommand" |> Binding.cmd (fun m -> Notification.InfoWithTimer "Title" m.Text (TimeSpan.FromSeconds 10.0)
                                                 |> Notifications.Msg.AddNotification |> NotificationMsg |> Local)
        "Notifications"
        |> Binding.subModel ((fun m -> m.Notifications), snd, Local << NotificationMsg, Notifications.Program.Bindings)
    ]

    static member GetLifecycleEventsSubscription (
        addHandlers : Action<Action<exn, string, bool, Action<bool>>,
                             Action<DateTimeOffset, Action>,
                             Action<obj>,
                             Action<Action>,
                             Action<Action>>)
        : Model -> Cmd<ProgramMessage<RootMsg, Msg>> =
        fun (_ : Model) ->
            fun (dispatch : Dispatch<ProgramMessage<RootMsg, Msg>>) ->
                addHandlers.Invoke (
                    (fun ex message handled setHandled ->
                        UnhandledException (ex, message) |> Local |> dispatch;
                        setHandled.Invoke true),
                    (fun deadline completed -> Suspend (deadline, completed) |> Local |> dispatch),
                    (fun o -> Resuming  |> Local |> dispatch),
                    (fun completed -> (EnteredBackground completed) |> Local |> dispatch),
                    (fun completed -> (LeavingBackground completed) |> Local |> dispatch))
            |> Cmd.ofSub

    static member GetNetworkStatusSubscription (addNetworkStatusHandler : Action<Action<NetworkConnectivityLevel>>) : Model -> Cmd<ProgramMessage<RootMsg, Msg>> =
        fun (_ : Model) ->
            fun (dispatch : Dispatch<ProgramMessage<RootMsg, Msg>>) ->
                addNetworkStatusHandler.Invoke (fun connectivityLevel -> NetworkChanged connectivityLevel |> Local |> dispatch)
            |> Cmd.ofSub

    member _.ServiceProvider = scope.ServiceProvider

    member val Program = Program.mkProgramUno init updateRoot bindings
                         |> Program.withConsoleTrace
