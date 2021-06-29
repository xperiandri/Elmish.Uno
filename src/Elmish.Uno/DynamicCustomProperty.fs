namespace Elmish.Uno

open System
#if __UWP__
open Microsoft.UI.Xaml.Data;

// TODO: investigate why nulls come to the constructor instead of None
type DynamicCustomProperty<'TTarget, 'TValue> (
  //  name : string,
  //  getter : Func<'TValue>,
  //  [<Optional; DefaultParameterValue(null)>]
  //  setter : Action<'TValue>,
  //  [<Optional; DefaultParameterValue(null)>]
  //  indexGetter : Func<obj, 'TValue>,
  //  [<Optional; DefaultParameterValue(null)>]
  //  indexSetter : Action<obj, 'TValue>) =

  //new (
      name : string,
      ?getter : 'TTarget -> 'TValue,
      ?setter : 'TTarget -> 'TValue -> unit,
      ?indexGetter : 'TTarget -> obj -> 'TValue,
      ?indexSetter : 'TTarget -> obj -> 'TValue -> unit) =
    //let setter' = defaultArg setter null
    //let indexGetter' = defaultArg indexGetter null
    //let indexSetter' = defaultArg indexSetter null
    //DynamicCustomProperty<'TValue>(name, getter, setter', indexGetter', indexSetter')

  member _.Getter = getter
  member _.Setter = setter
  member _.IndexGetter = indexGetter
  member _.IndexSetter = indexSetter

  interface ICustomProperty with

    member _.GetValue (target : obj) =
      match getter with Some getter -> getter (target :?> 'TTarget) |> box | None -> null
    member _.SetValue (target : obj, value : obj) =
      match setter with Some setter -> setter (target :?> 'TTarget) (value :?> 'TValue) | None -> ()
    member _.GetIndexedValue(target : obj, index : obj) =
      match indexGetter with Some indexGetter -> indexGetter (target :?> 'TTarget) index |> box | None -> null
    member _.SetIndexedValue(target : obj, value : obj, index : obj) =
      match indexSetter with Some indexSetter -> indexSetter (target :?> 'TTarget) index (value :?> 'TValue) | None -> ()

    member _.CanRead = getter.IsSome || indexGetter.IsSome
    member _.CanWrite = setter.IsSome || indexSetter.IsSome
    member _.Name = name
    member _.Type = typeof<'TValue>
#endif
