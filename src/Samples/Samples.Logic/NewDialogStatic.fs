namespace rec Elmish.Uno.Samples.NewDialogStatic

open System
open Elmish
open Elmish.Uno
open Microsoft.UI.Xaml
open Microsoft.UI.Xaml.Controls
open FSharp.Core

//open Elmish.Uno.Samples.NewDialogStatic.Dialog1
//open Elmish.Uno.Samples.NewDialogStatic.Dialog2

type Model = {
    Dialog1: WindowState<string>
    Dialog2: Dialog2.Dialog2 voption
} with
    static member Init ()
      = { Dialog1 = WindowState.Closed; Dialog2 = ValueNone }

type Msg =
    | Dialog1Show of Param : string
    | Dialog1Close
    | Dialog1Msg of Dialog1.Dialog1Msg
    | Dialog1SetInput of string
    | Dialog2Show
    | Dialog2Close
    | Dialog2Msg of Dialog2.Dialog2Msg

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Dialog1 =
    //let get m = m.Dialog1
    //let set v m = { m with Dialog1 = v }
    //let map = AutoOpenDialog.map get set
    let name = ()

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Dialog2 =
    let mapOutMsg msg =
        match msg with
        | Dialog2.Dialog2OutMsg.Close -> Dialog2Close
    let mapInOutMsg = InOut.cata Dialog2Msg mapOutMsg

module Program =

    let init () = Model.Init (), Cmd.none

    let update msg (m: Model) =
        match msg with
        | Dialog1Show _ -> { m with Dialog1 = WindowState.Visible (Dialog1.Program.init ()) }, Cmd.none
        | Dialog1Close -> { m with Dialog1 = WindowState.Closed }, Cmd.none
        | Dialog1SetInput s -> { m with Dialog1 = WindowState.set s m.Dialog1 }, Cmd.none
        | Dialog2Show -> { m with Dialog2 = ValueSome (Dialog2.Program.init ()) }, Cmd.none
        | Dialog2Close -> { m with Dialog2 = ValueNone }, Cmd.none
        | Dialog2Msg msg when m.Dialog2.IsSome ->
            let dialogM, dialogCmd = Dialog2.Program.update msg m.Dialog2.Value
            { m with Dialog2 = ValueSome dialogM }, dialogCmd |> Cmd.map Dialog2.mapOutMsg
        | Dialog2Msg _ -> m, Cmd.none

module Bindings =

    let private viewModel = Unchecked.defaultof<NewDialogStaticViewModel>

    let dialog1ShowBinding =
        BindingT.cmdParamIf (Dialog1Show, (fun p m -> true)) (nameof viewModel.Dialog1Show)

    let dialog1CloseBinding =
        BindingT.cmd Dialog1Close (nameof viewModel.Dialog1Close)

    let dialog2ShowBinding =
        BindingT.cmd Dialog2Show (nameof viewModel.Dialog2Show)

    let dialog1Binding (createDialog : unit -> ContentDialog) =
        BindingT.subModelDialog(Dialog1.Dialog1ViewModel, (_.Dialog1 >> WindowState.map string),Dialog1Msg, createDialog, Dialog1Close) (nameof viewModel.Dialog1)

    let dialog2Binding (createDialog : unit -> ContentDialog) =
        BindingT.subModelDialog(Dialog2.Dialog2ViewModel, (_.Dialog2 >> WindowState.ofValueOption), Dialog2Msg, createDialog) (nameof viewModel.Dialog2)

type NewDialogStaticViewModel (createDialog1 : Func<ContentDialog>, createDialog2: Func<ContentDialog>, dispatcher) as vm =
    inherit ViewModelBase<Model, Msg>(
        let program = UnoProgram.mkProgramT Program.init Program.update (fun _ -> vm :> IViewModel<Model, Msg>) in
        UnoProgram.createVmArgs dispatcher (Func<_> (fun () -> vm :> IViewModel<Model, Msg>)) program
    )

    do let _ = vm.Dialog1 in ()
    do let _ = vm.Dialog2 in ()

    member _.Dialog1Show = base.Get(Bindings.dialog1ShowBinding)
    member _.Dialog1Close = base.Get(Bindings.dialog1CloseBinding)
    member _.Dialog2Show = base.Get(Bindings.dialog2ShowBinding)

    member _.Dialog1 = base.Get(Bindings.dialog1Binding (fun () -> createDialog1.Invoke()))
    member _.Dialog2 = base.Get(Bindings.dialog2Binding (fun () -> createDialog2.Invoke()))

    static member DesignInstance = NewDialogStaticViewModel (Unchecked.defaultof<_>,Unchecked.defaultof<_>,Unchecked.defaultof<_>)
