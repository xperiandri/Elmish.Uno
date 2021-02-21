[<RequireQualifiedAccess>]
module Elmish.Uno.Program

open System.Windows
open Elmish


/// Same as mkSimple, but with a signature adapted for Elmish.Uno.
let mkSimpleUno
    (init: 'arg -> 'model)
    (update: 'msg  -> 'model -> 'model)
    (bindings: Binding<'model, 'msg> list) =
  Program.mkSimple init update (fun _ _ -> bindings)


/// Same as mkProgram, but with a signature adapted for Elmish.Uno.
let mkProgramUno
    (init: 'arg -> 'model * Cmd<'msg>)
    (update: 'msg  -> 'model -> 'model * Cmd<'msg>)
    (bindings: Binding<'model, 'msg> list) =
  Program.mkProgram init update (fun _ _ -> bindings)


/// Same as mkProgramUno, except that init and update doesn't return Cmd<'msg>
/// directly, but instead return a CmdMsg discriminated union that is converted
/// to Cmd<'msg> using toCmd. This means that the init and update functions
/// return only data, and thus are easier to unit test. The CmdMsg pattern is
/// general; this is just a trivial convenience function that automatically
/// converts CmdMsg to Cmd<'msg> for you in inint and update
let mkProgramUnoWithCmdMsg
    (init: unit -> 'model * 'cmdMsg list)
    (update: 'msg -> 'model -> 'model * 'cmdMsg list)
    (bindings: Binding<'model, 'msg> list)
    (toCmd: 'cmdMsg -> Cmd<'msg>) =
  let convert (model, cmdMsgs) =
    model, (cmdMsgs |> List.map toCmd |> Cmd.batch)
  mkProgramUno
    (init >> convert)
    (fun msg model -> update msg model |> convert)
    bindings


/// Traces all updates using System.Diagnostics.Debug.WriteLine.
let withDebugTrace program =
  program |> Program.withTrace (fun msg model ->
    System.Diagnostics.Debug.WriteLine(sprintf "New message: %A" msg)
    System.Diagnostics.Debug.WriteLine(sprintf "Updated state: %A" model)
  )
