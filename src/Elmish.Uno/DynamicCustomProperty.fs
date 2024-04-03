namespace Elmish.Uno

open System
open System.Runtime.InteropServices
open Microsoft.UI.Xaml.Data;

// TODO: investigate why nulls come to the constructor instead of None
/// <summary>
/// Implementation of dynamic property required by WinRT to do bindings.
/// </summary>
/// <typeparam name="target">Target object type from which to get and to which to set a property value.</typeparam>
/// <typeparam name="value">Value type.</typeparam>
type DynamicCustomProperty<'target, 'value> (
    name : string,
    getter : Func<'target, 'value>,
    [<Optional>] setter : Action<'target, 'value>,
    [<Optional>] indexGetter : Func<'target, obj, 'value>,
    [<Optional>] indexSetter : Action<'target, 'value, obj>
) =

  //new (
      //name : string,
      //?getter : 'target -> 'value,
      //?setter : 'value -> 'target -> unit,
      //?indexGetter : obj -> 'target -> 'value,
      //?indexSetter : 'value -> obj -> 'target -> unit) =
    //let setter' = defaultArg setter null
    //let indexGetter' = defaultArg indexGetter null
    //let indexSetter' = defaultArg indexSetter null
    //DynamicCustomProperty<'TValue>(name, getter, setter', indexGetter', indexSetter')

  /// <summary>
  /// Property getter function.
  /// </summary>
  member _.Getter = getter
  /// <summary>
  /// Property setter function
  /// </summary>
  member _.Setter = setter
  /// <summary>
  /// Indexer getter function
  /// </summary>
  member _.IndexGetter = indexGetter
  /// <summary>
  /// Indexer setter function
  /// </summary>
  member _.IndexSetter = indexSetter

  interface ICustomProperty with

    member _.GetValue (target : obj) =
      let target = target :?> 'target
      match getter with null -> null | _ -> getter.Invoke target |> box
    member _.SetValue (target : obj, value : obj) =
      let target = target :?> 'target
      let value = value :?> 'value
      match setter with null -> () | _ -> setter.Invoke (target, value)
    member _.GetIndexedValue(target : obj, index : obj) =
      let target = target :?> 'target
      match indexGetter with null -> null | _ -> indexGetter.Invoke(target, index) |> box
    member _.SetIndexedValue(target : obj, value : obj, index : obj) =
      let target = target :?> 'target
      let value = value :?> 'value
      match indexSetter with null -> () | _ -> indexSetter.Invoke(target, value, index)

    member _.CanRead = getter <> null || indexGetter <> null
    member _.CanWrite = setter <> null || indexSetter <> null
    member _.Name = name
    member _.Type = typeof<'value>
