namespace SolutionTemplate.Elmish

open System.Runtime.CompilerServices
open Elmish

[<AutoOpen>]
module State =

    open Microsoft.FSharp.Reflection

    /// Returns the case name of the object with union type 'ty.
    let getUnionCaseName (x: 'a) =
        match FSharpValue.GetUnionFields(x, typeof<'a>) with
        | case, _ -> case.Name

[<AutoOpen>]
module Dispatching =

    open System
    open FSharp.Control.Reactive

    let asDispatchWrapper<'msg>
        (configure: IObservable<'msg> -> IObservable<'msg>)
        (dispatch: Dispatch<'msg>)
        : Dispatch<'msg> =
        let subject = Subject.broadcast
        subject |> configure |> Observable.add dispatch
        subject.OnNext >> async.Return >> Async.Start

    let throttle<'msg> =
        Observable.throttle (TimeSpan.FromMilliseconds 500.)
        |> asDispatchWrapper<'msg>

[<AbstractClass; Extension>]
type ProgramExtensions =

    [<Extension>]
    static member WithSetState (program, setState) =
        Program.withSetState setState program

    [<Extension>]
    static member WithSubscription (program, subscribe) =
        Program.withSubscription subscribe program

    //[<Extension>]
    //static member WithSetState (program, setState : Action<'model, Dispatch<'msg>>) =
    //    Program.withSetState (setState |> FuncConvert.FromAction) program

    //[<Extension>]
    //static member WithSubscription (program, subscribe : Func<'model, Cmd<'msg>>) =
    //    Program.withSubscription (subscribe |> FuncConvert.FromFunc) program
