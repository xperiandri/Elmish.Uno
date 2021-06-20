namespace Elmish.Uno

open System
#if __UWP__
open Microsoft.UI.Xaml.Data;

// TODO: investigate why nulls come to the constructor instead of None
type DynamicCustomProperty<'TValue> (
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
      ?getter : unit -> 'TValue,
      ?setter : 'TValue -> unit,
      ?indexGetter : obj -> 'TValue,
      ?indexSetter : obj -> 'TValue -> unit) =
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
      match getter with Some getter -> getter() |> box | None -> null
    member _.SetValue (target : obj, value : obj) =
      match setter with Some setter -> setter(value :?> 'TValue) | None -> ()
    member _.GetIndexedValue(target : obj, index : obj) =
      match indexGetter with Some indexGetter -> indexGetter index |> box | None -> null
    member _.SetIndexedValue(target : obj, value : obj, index : obj) =
      match indexSetter with Some indexSetter -> indexSetter index (value :?> 'TValue) | None -> ()

    member _.CanRead = getter.IsSome || indexGetter.IsSome
    member _.CanWrite = setter.IsSome || indexSetter.IsSome
    member _.Name = name
    member _.Type = typeof<'TValue>
#endif
