namespace SolutionTemplate.Pages

[<AutoOpen>]
module Pages =

    [<Literal>]
    let Main = "Main"

open System

type NavigationError (getHandled : Func<bool>, setHandled : Action<bool>, ex, sourcePageType) =

    member _.Handled
        with get () : bool = getHandled.Invoke ()
        and set (value : bool) : unit = setHandled.Invoke value
    member _.Exception : Exception = ex
    member _.SourcePageType : Type = sourcePageType
