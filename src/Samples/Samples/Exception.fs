[<AutoOpen>]
[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module System.Exception

open System
open System.Runtime.ExceptionServices

// Useful for reraising exceptions under an async{...} context
// See this for more details: https://github.com/fsharp/fslang-suggestions/issues/660
type Exception with
    member __.Reraise () =
        (ExceptionDispatchInfo.Capture __).Throw ()
        Unchecked.defaultof<_>
