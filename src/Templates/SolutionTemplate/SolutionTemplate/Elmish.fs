namespace SolutionTemplate.Elmish1

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
    open Elmish
    open FSharp.Control.Reactive

    let asDispatchWrapper<'msg>
        (configure: IObservable<'msg> -> IObservable<'msg>)
        (dispatch: Dispatch<'msg>)
        : Dispatch<'msg> =
        let subject = Subject.broadcast
        subject |> configure |> Observable.add dispatch
        fun msg -> async.Return(subject.OnNext msg) |> Async.Start

    let throttle<'msg> =
        Observable.throttle (TimeSpan.FromMilliseconds 500.)
        |> asDispatchWrapper<'msg>

