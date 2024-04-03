namespace SolutionTemplate

module ValueOption =
    let toOption value =
        match value with | ValueSome v -> Some v | _ -> None

module Option =
    let toVOption value =
        match value with | Some v -> ValueSome v | _ -> ValueNone

[<AutoOpen>]
module ValueTuple =

    let fstv struct (a,_) =  a
    let sndv struct (_,b) =  b
