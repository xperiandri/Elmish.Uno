[<AutoOpen>]
module internal Elmish.Uno.InternalTypes

open System
open System.Windows.Input

type Command(execute, canExecute) as this =

  let canExecuteChanged = Event<EventHandler,EventArgs>()
  let handler = EventHandler(fun _ _ -> this.RaiseCanExecuteChanged())

  // Strong handler must be maintained
  member private x._Handler = handler

  member x.RaiseCanExecuteChanged () = canExecuteChanged.Trigger(x,EventArgs.Empty)

  interface ICommand with
    [<CLIEvent>]
    member x.CanExecuteChanged = canExecuteChanged.Publish
    member x.CanExecute p = canExecute p
    member x.Execute p = execute p
