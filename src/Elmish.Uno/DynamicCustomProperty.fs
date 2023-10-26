namespace Elmish.Uno

open System
#if __UWP__
open Microsoft.UI.Xaml.Data;

// TODO: investigate why nulls come to the constructor instead of None
/// <summary>
/// Implementation of dynamic property required by WinRT to do bindings.
/// </summary>
/// <typeparam name="TTarget">Target object type from which to get and to which to set a property value.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
type DynamicCustomProperty<'target, 'value> (
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
      ?getter : 'target -> 'value,
      ?setter : 'value -> 'target -> unit,
      ?indexGetter : obj -> 'target -> 'value,
      ?indexSetter : 'value -> obj -> 'target -> unit) =
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
      match getter with Some getter -> getter target |> box | None -> null
    member _.SetValue (target : obj, value : obj) =
      let target = target :?> 'target
      match setter with Some setter -> setter (value :?> 'value) target | None -> ()
    member _.GetIndexedValue(target : obj, index : obj) =
      let target = target :?> 'target
      match indexGetter with Some indexGetter -> indexGetter index target |> box | None -> null
    member _.SetIndexedValue(target : obj, value : obj, index : obj) =
      let target = target :?> 'target
      match indexSetter with Some indexSetter -> indexSetter (value :?> 'value) index target | None -> ()

    member _.CanRead = getter.IsSome || indexGetter.IsSome
    member _.CanWrite = setter.IsSome || indexSetter.IsSome
    member _.Name = name
    member _.Type = typeof<'value>

#endif
