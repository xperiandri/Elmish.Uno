namespace Elmish.Uno

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.ComponentModel
open System.Dynamic
open System.Reflection
open Microsoft.FSharp.Reflection
open FSharp.Collections.Immutable

open Elmish
open Elmish.Uno

#if __UWP__
open Microsoft.UI.Xaml.Data
#endif


[<AutoOpen>]
module internal ViewModelHelpers =

  let updateObservableCollection
        (create: 's -> 'id -> 't)
        update
        (target: ObservableCollection<'t>, getTargetId: 't -> 'id)
        (source: 's array, getSourceId: 's -> 'id) =

    let kvp (k, v) = KeyValuePair<_,_>(k, v)

    let targetIds = target |> Seq.map getTargetId |> Seq.toFlatList
    let sourceIds = source |> Seq.map getSourceId |> Seq.toFlatList

    let targetIdsSet = targetIds |> HashSet.ofSeq
    let sourceIdsSet = sourceIds |> HashSet.ofSeq

    let targetIdsMap = targetIds |> Seq.mapi (fun i id -> kvp(id, i)) |> Seq.toHashMap

    let removes =
      targetIdsSet.Except sourceIdsSet
      |> Seq.map (fun id -> targetIdsMap.[id])
      |> Seq.sortDescending
      |> Seq.toFlatList

    removes |> Seq.iter target.RemoveAt

    let sourceIdsMap = sourceIds |> Seq.mapi (fun i id -> kvp(id, i)) |> Seq.toHashMap
    let adds =
      sourceIdsSet.Except targetIdsSet
      |> Seq.map (fun id -> id, sourceIdsMap.[id])
      |> Seq.sortBy (fun (_, idx) -> idx)
      |> Seq.toFlatList

    adds |> Seq.iter (fun (id, index) -> target.Insert(index, create (source.[index]) id))

    sourceIds
    |> Seq.iter (fun id ->
      let newIdx = sourceIdsMap.[id]
      let oldIdx = target |> Seq.mapi (fun i t -> i, getTargetId t) |> Seq.where (fun (_, tId) -> id = tId) |> Seq.map fst |> Seq.head
      if newIdx <> oldIdx then target.Move(oldIdx, newIdx)
      update newIdx
    )


type internal OneWayBinding<'model, 'a> = {
  Get: 'model -> 'a
}

type internal OneWayLazyBinding<'model, 'a, 'b> = {
  Get: 'model -> 'a
  Equals: 'a -> 'a -> bool
  Map: 'a -> 'b
}

type internal OneWaySeqBinding<'model, 'a, 'b, 'id> = {
  Get: 'model -> 'a
  Equals: 'a -> 'a -> bool
  Map: 'a -> 'b seq
  GetId: 'b -> 'id
  ItemEquals: 'b -> 'b -> bool
  Values: ObservableCollection<'b>
}

type internal TwoWayBinding<'model, 'msg, 'a> = {
  Get: 'model -> 'a
  Set: 'a -> 'model -> unit
}

type internal TwoWayValidateBinding<'model, 'msg, 'a, 'e, 'errorId> = {
  Get: 'model -> 'a
  Set: 'a -> 'model -> unit
  Validate: 'model -> obj array
  GetErrorId: 'e -> 'errorId
  ErrorItemEquals: 'e -> 'e -> bool
  Errors : ObservableCollection<'e> Lazy
}

type internal CmdBinding<'model, 'msg> = {
  Cmd: Command
  CanExec: 'model -> bool
}

type internal SubModelBinding<'model, 'msg, 'bindingModel, 'bindingMsg> = {
  GetModel: 'model -> 'bindingModel voption
  GetBindings: unit -> Binding<'bindingModel, 'bindingMsg> list
  ToMsg: 'bindingMsg -> 'msg
  Sticky: bool
  Vm: ViewModel<'bindingModel, 'bindingMsg> voption ref
}

and internal SubModelSeqBinding<'model, 'msg, 'bindingModel, 'bindingMsg, 'id> = {
  GetModels: 'model -> 'bindingModel seq
  GetId: 'bindingModel -> 'id
  GetBindings: unit -> Binding<'bindingModel, 'bindingMsg> list
  ToMsg: 'id * 'bindingMsg -> 'msg
  Vms: ObservableCollection<ViewModel<'bindingModel, 'bindingMsg>>
}

and internal SubModelSelectedItemBinding<'model, 'msg, 'bindingModel, 'bindingMsg, 'id> = {
  Get: 'model -> 'id voption
  Set: 'id voption -> 'model -> unit
  SubModelSeqBinding: SubModelSeqBinding<'model, 'msg, obj, obj, obj>
}

and internal CachedBinding<'model, 'msg, 'value> = {
  Binding: VmBinding<'model, 'msg>
  Cache: 'value option ref
}


/// Represents all necessary data used in an active binding.
and internal VmBinding<'model, 'msg> =
  | OneWay of OneWayBinding<'model, obj>
  | OneWayLazy of OneWayLazyBinding<'model, obj, obj>
  | OneWaySeq of OneWaySeqBinding<'model, obj, obj, obj>
  | TwoWay of TwoWayBinding<'model, 'msg, obj>
  | TwoWayValidate of TwoWayValidateBinding<'model, 'msg, obj, obj, obj>
  | Cmd of CmdBinding<'model, 'msg>
  | CmdParam of cmd: Command
  | SubModel of SubModelBinding<'model, 'msg, obj, obj>
  | SubModelSeq of SubModelSeqBinding<'model, 'msg, obj, obj, obj>
  | SubModelSelectedItem of SubModelSelectedItemBinding<'model, 'msg, obj, obj, obj>
  | Cached of CachedBinding<'model, 'msg, obj>


and [<AllowNullLiteral>] internal ViewModel<'model, 'msg>
      ( initialModel: 'model,
        dispatch: 'msg -> unit,
        bindings: Binding<'model, 'msg> list,
        config: ElmConfig,
        propNameChain: string)
      as this =
  inherit DynamicObject()

  let mutable currentModel = initialModel

  let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()
  let errorsChanged = DelegateEvent<EventHandler<DataErrorsChangedEventArgs>>()
  let modelTypeChanged = Event<EventHandler, EventArgs>()

  static let multicastFiled = typeof<Event<EventHandler, EventArgs>>.GetField("multicast", BindingFlags.NonPublic ||| BindingFlags.Instance)
  let getDelegateFromEvent (event : Event<EventHandler, EventArgs>) =
    let ``delegate`` = multicastFiled.GetValue event
    match ``delegate`` with
    | :? EventHandler as handler -> handler
    | _ -> null

  /// Error messages keyed by property name.
  let errors = Dictionary<string, obj ICollection>()


  let withCaching b = Cached { Binding = b; Cache = ref None }


  let log fmt =
    let innerLog (str: string) =
      if config.LogConsole then Console.WriteLine(str)
      if config.LogTrace then Diagnostics.Trace.WriteLine(str)
    Printf.kprintf innerLog fmt

  let getPropChainFor bindingName =
    sprintf "%s.%s" propNameChain bindingName

  let getPropChainForItem collectionBindingName itemId =
    sprintf "%s.%s.%s" propNameChain collectionBindingName itemId

  let notifyPropertyChanged propName =
    log "[%s] PropertyChanged \"%s\"" propNameChain propName
    propertyChanged.Trigger(this, PropertyChangedEventArgs propName)

  let raiseCanExecuteChanged (cmd: Command) =
    cmd.RaiseCanExecuteChanged ()

  let setError propErrors propName =
    match errors.TryGetValue propName with
    | true, _ -> ()
    | _ ->
       log "[%s] ErrorsChanged \"%s\"" propNameChain propName
       errors.[propName] <- propErrors
       errorsChanged.Trigger([| box this; box <| DataErrorsChangedEventArgs propName |])

  let removeError propName =
    if errors.Remove propName then
      log "[%s] ErrorsChanged \"%s\"" propNameChain propName
      errorsChanged.Trigger([| box this; box <| DataErrorsChangedEventArgs propName |])

  let rec updateValidationError model name = function
    | TwoWayValidate { Validate = validate; Errors = errors; GetErrorId = getErrorId; ErrorItemEquals = errorItemEquals } ->
        match validate model with
        | [||] ->
          errors.Value.Clear()
          removeError name
        | propErrors ->
          let create v _ = v
          let update idx =
            let oldVal = errors.Value.[idx]
            let newVal = propErrors.[idx]
            if not (errorItemEquals newVal oldVal) then
              errors.Value.[idx] <- newVal
          setError errors.Value name
          updateObservableCollection create update (errors.Value, getErrorId) (propErrors, getErrorId)
        notifyPropertyChanged "HasErrors"
    | OneWay _
    | OneWayLazy _
    | OneWaySeq _
    | TwoWay _
    | Cmd _
    | CmdParam _
    | SubModel _
    | SubModelSeq _
    | SubModelSelectedItem _ -> ()
    | Cached b -> updateValidationError model name b.Binding

  let measure name callName f =
    if not config.Measure then f
    else
      fun x ->
        let sw = System.Diagnostics.Stopwatch.StartNew ()
        let r = f x
        sw.Stop ()
        if sw.ElapsedMilliseconds >= int64 config.MeasureLimitMs then
          log "[%s] %s (%ims): %s" propNameChain callName sw.ElapsedMilliseconds name
        r

  let measure2 name callName f =
    if not config.Measure then f
    else fun x -> measure name callName (f x)

(*
  let showNewWindow
      (winRef: WeakReference<Window>)
      (getWindow: 'model -> Dispatch<'msg> -> Window)
      dataContext
      isDialog
      (onCloseRequested: unit -> unit)
      (preventClose: bool ref)
      initialVisibility =
    let win = getWindow currentModel dispatch
    winRef.SetTarget win
    win.Dispatcher.Invoke(fun () ->
      let guiCtx = System.Threading.SynchronizationContext.Current
      async {
        win.DataContext <- dataContext
        win.Closing.Add(fun ev ->
          ev.Cancel <- !preventClose
          async {
            do! Async.SwitchToThreadPool()
            onCloseRequested ()
          } |> Async.StartImmediate
        )
        do! Async.SwitchToContext guiCtx
        if isDialog
        then win.ShowDialog () |> ignore
        else win.Visibility <- initialVisibility
      } |> Async.StartImmediate
    )
*)

  let initializeBinding name bindingData getInitializedBindingByName =
    match bindingData with
    | OneWayData d ->
        Some <| OneWay {
          Get = measure name "get" d.Get }
    | OneWayLazyData d ->
        let get = measure name "get" d.Get
        let map = measure name "map" d.Map
        OneWayLazy {
          Get = get
          Map = map
          Equals = measure2 name "equals" d.Equals
        } |> withCaching |> Some
    | OneWaySeqLazyData d ->
        let get = measure name "get" d.Get
        let map = measure name "map" d.Map
        let getId = measure name "getId" d.GetId
        let values = ObservableCollection(initialModel |> get |> map)
        Some <| OneWaySeq {
          Get = get
          Map = map
          Equals = measure2 name "equals" d.Equals
          GetId = getId
          ItemEquals = measure2 name "itemEquals" d.ItemEquals
          Values = values }
    | TwoWayData d ->
        let set = measure2 name "set" d.Set
        let dispatch' = d.WrapDispatch dispatch
        Some <| TwoWay {
          Get = measure name "get" d.Get
          Set = fun obj m -> set obj m |> dispatch' }
    | TwoWayValidateData d ->
        let set = measure2 name "set" d.Set
        let getErrorId = measure name "getErrorId" d.GetErrorId
        let dispatch' = d.WrapDispatch dispatch
        Some <| TwoWayValidate {
          Get = measure name "get" d.Get
          GetErrorId = getErrorId
          ErrorItemEquals =  measure2 name "errorItemEquals" d.ErrorItemEquals
          Set = fun obj m -> set obj m |> dispatch'
          Validate = measure name "validate" d.Validate
          Errors = Lazy<_>()   }
    | CmdData d ->
        let exec = measure name "exec" d.Exec
        let canExec = measure name "canExec" d.CanExec
        let dispatch' = d.WrapDispatch dispatch
        let execute _ = exec currentModel |> ValueOption.iter dispatch'
        let canExecute _ = canExec currentModel
        Some <| Cmd {
          Cmd = Command(execute, canExecute)
          CanExec = canExec }
    | CmdParamData d ->
        let exec = measure2 name "exec" d.Exec
        let canExec = measure2 name "canExec" d.CanExec
        let dispatch' = d.WrapDispatch dispatch
        let execute param = exec param currentModel |> ValueOption.iter dispatch'
        let canExecute param = canExec param currentModel
        Some <| CmdParam (Command(execute, canExecute))
    | SubModelData d ->
        let getModel = measure name "getSubModel" d.GetModel
        let getBindings = measure name "bindings" d.GetBindings
        let toMsg = measure name "toMsg" d.ToMsg
        match getModel initialModel with
        | ValueNone ->
            Some <| SubModel {
              GetModel = getModel
              GetBindings = getBindings
              ToMsg = toMsg
              Sticky = d.Sticky
              Vm = ref ValueNone }
        | ValueSome m ->
            let chain = getPropChainFor name
            let vm = this.Create(m, toMsg >> dispatch, getBindings (), config, chain)
            Some <| SubModel {
              GetModel = getModel
              GetBindings = getBindings
              ToMsg = toMsg
              Sticky = d.Sticky
              Vm = ref <| ValueSome vm }
    | SubModelSeqData d ->
        let getModels = measure name "getSubModels" d.GetModels
        let getId = measure name "getId" d.GetId
        let getBindings = measure name "bindings" d.GetBindings
        let toMsg = measure name "toMsg" d.ToMsg
        let vms =
          getModels initialModel
          |> Seq.map (fun m ->
               let chain = getPropChainForItem name (getId m |> string)
               this.Create(m, (fun msg -> toMsg (getId m, msg) |> dispatch), getBindings (), config, chain)
          )
          |> ObservableCollection
        Some <| SubModelSeq {
          GetModels = getModels
          GetId = getId
          GetBindings = getBindings
          ToMsg = toMsg
          Vms = vms }
    | SubModelSelectedItemData d ->
        match getInitializedBindingByName d.SubModelSeqBindingName with
        | Some (SubModelSeq b) ->
          let get = measure name "get" d.Get
          let set = measure2 name "set" d.Set
          let dispatch' = d.WrapDispatch dispatch
          SubModelSelectedItem {
            Get = get
            Set = fun obj m -> set obj m |> dispatch'
            SubModelSeqBinding = b
          } |> withCaching |> Some
        | _ ->
          log "subModelSelectedItem binding referenced binding '%s', but no compatible binding was found with that name" d.SubModelSeqBindingName
          None

  let bindings =
    lazy
        log "[%s] Initializing bindings" propNameChain
        let dict = Dictionary<string, VmBinding<'model, 'msg>>(bindings.Length)
        let dictAsFunc name =
          match dict.TryGetValue name with
          | true, b -> Some b
          | _ -> None
        let sortedBindings = bindings |> List.sortWith Binding.subModelSelectedItemLast
        for b in sortedBindings do
          if dict.ContainsKey b.Name then
            log "Binding name '%s' is duplicated. Only the first occurrence will be used." b.Name
          else
            initializeBinding b.Name b.Data dictAsFunc
            |> Option.iter (fun binding ->
              dict.Add(b.Name, binding)
              updateValidationError initialModel b.Name binding)
        dict :> IReadOnlyDictionary<string, VmBinding<'model, 'msg>>

  /// Returns the command associated with a command binding if the command's
  /// CanExecuteChanged should be triggered.
  let rec getCmdIfCanExecChanged newModel = function
    | OneWay _
    | OneWayLazy _
    | OneWaySeq _
    | TwoWay _
    | TwoWayValidate _
    | SubModel _
    | SubModelSeq _
    | SubModelSelectedItem _ ->
        None
    | Cmd { Cmd = cmd; CanExec = canExec } ->
        if canExec newModel = canExec currentModel
        then None
        else Some cmd
    | CmdParam cmd ->
        Some cmd
    | Cached b -> getCmdIfCanExecChanged newModel b.Binding

  let rec tryGetMember model = function
    | OneWay { Get = get }
    | TwoWay { Get = get }
    | TwoWayValidate { Get = get } ->
        get model
    | OneWayLazy b ->
        model |> b.Get |> b.Map
    | OneWaySeq { Values = vals } ->
        box vals
    | Cmd { Cmd = cmd }
    | CmdParam cmd ->
        box cmd
    | SubModel { Vm = vm } -> !vm |> ValueOption.toObj |> box
    | SubModelSeq { Vms = vms } -> box vms
    | SubModelSelectedItem b ->
        let selectedId = b.Get model
        let selected =
          b.SubModelSeqBinding.Vms
          |> Seq.tryFind (fun (vm: ViewModel<obj, obj>) ->
            selectedId = ValueSome (b.SubModelSeqBinding.GetId vm.CurrentModel))
        log "[%s] Setting selected VM to %A"
          propNameChain
          (selected |> Option.map (fun vm -> b.SubModelSeqBinding.GetId vm.CurrentModel))
        selected |> Option.toObj |> box
    | Cached b ->
        match !b.Cache with
        | Some v -> v
        | None ->
            let v = tryGetMember model b.Binding
            b.Cache := Some v
            v

  let rec canSetMember = function
    | TwoWay _
    | TwoWayValidate _
    | SubModelSelectedItem _ ->
        true
    | Cached b ->
        let successful = canSetMember b.Binding
        successful
    | OneWay _
    | OneWayLazy _
    | OneWaySeq _
    | Cmd _
    | CmdParam _
    | SubModel _
    | SubModelSeq _ ->
        false

  let rec trySetMember model (value: obj) = function
    | TwoWay { Set = set }
    | TwoWayValidate { Set = set } ->
        set value model
        true
    | SubModelSelectedItem b ->
        let id =
          (value :?> ViewModel<obj, obj>)
          |> ValueOption.ofObj
          |> ValueOption.map (fun vm -> b.SubModelSeqBinding.GetId vm.CurrentModel)
        b.Set id model
        true
    | Cached b ->
        let successful = trySetMember model value b.Binding
        if successful then
          b.Cache := None  // TODO #185: write test
        successful
    | OneWay _
    | OneWayLazy _
    | OneWaySeq _
    | Cmd _
    | CmdParam _
    | SubModel _
    | SubModelSeq _ ->
        false

  /// Updates the binding value (for relevant bindings) and returns a value
  /// indicating whether to trigger PropertyChanged for this binding
  member this.UpdateValue =
    let rec updateValue bindingName newModel = function
      | OneWay { Get = get }
      | TwoWay { Get = get }
      | TwoWayValidate { Get = get } ->
          get currentModel <> get newModel
      | OneWayLazy b ->
          not <| b.Equals (b.Get newModel) (b.Get currentModel)
      | OneWaySeq b ->
          let intermediate = b.Get newModel
          if not <| b.Equals intermediate (b.Get currentModel) then
            let create v _ = v
            let newVals = intermediate |> b.Map |> Seq.toArray
            let update idx =
              let oldVal = b.Values.[idx]
              let newVal = newVals.[idx]
              if not (b.ItemEquals newVal oldVal) then
                b.Values.[idx] <- newVal
            updateObservableCollection create update (b.Values, b.GetId) (newVals, b.GetId)
          false
      | Cmd _
      | CmdParam _ ->
          false
      | SubModel b ->
        match !b.Vm, b.GetModel newModel with
        | ValueNone, ValueNone -> false
        | ValueSome _, ValueNone ->
            if b.Sticky then false
            else
              b.Vm := ValueNone
              true
        | ValueNone, ValueSome m ->
            b.Vm := ValueSome <| this.Create(m, b.ToMsg >> dispatch, b.GetBindings (), config, getPropChainFor bindingName)
            true
        | ValueSome vm, ValueSome m ->
            vm.UpdateModel m
            false
      | SubModelSeq b ->
          let getTargetId (vm: ViewModel<_, _>) = b.GetId vm.CurrentModel
          let create m id =
            let chain = getPropChainForItem bindingName (id |> string)
            this.Create(m, (fun msg -> b.ToMsg (id, msg) |> dispatch), b.GetBindings (), config, chain)
          let newSubModels = newModel |> b.GetModels |> Seq.toArray
          let update idx = b.Vms.[idx].UpdateModel newSubModels.[idx]
          updateObservableCollection create update (b.Vms, getTargetId) (newSubModels, b.GetId)
          false
      | SubModelSelectedItem b ->
          b.Get newModel <> b.Get currentModel
      | Cached b ->
          let valueChanged = updateValue bindingName newModel b.Binding
          if valueChanged then
            b.Cache := None
          valueChanged
    updateValue

  abstract Create : initialModel: obj * dispatch: (obj -> unit) * bindings: Binding<obj, obj> list * config: ElmConfig * propNameChain: string -> ViewModel<obj, obj>
  default __.Create (initialModel, dispatch, bindings, config, propNameChain) =
    ViewModel(initialModel, dispatch, bindings, config, propNameChain)
  member internal __.Bindings = bindings.Value
  member internal __.Dispatch = dispatch
  [<CLIEvent>]
  member val ModelTypeChanged = modelTypeChanged.Publish
  member this.CurrentModel
    with get () : 'model = currentModel
    and private set (newModel  : 'model) =
        // TODO:
        //if currentModel <> newModel then
            let oldModel = currentModel
            currentModel <- newModel
            notifyPropertyChanged "CurrentModel"
            let ``delegate`` = modelTypeChanged |> getDelegateFromEvent
            match ``delegate`` with
            | null -> ()
            | _ when ``delegate``.GetInvocationList () |> (not << Seq.isEmpty) ->
                let oldModelType = oldModel.GetType ()
                let newModelType = newModel.GetType ()
                match FSharpType.IsUnion newModelType, FSharpType.IsUnion newModelType with
                | true, true ->
                    let oldUnionCase, _ = FSharpValue.GetUnionFields(oldModel, oldModelType)
                    let newUnionCase, _ = FSharpValue.GetUnionFields(newModel, newModelType)
                    if oldUnionCase.DeclaringType <> newUnionCase.DeclaringType
                    || oldUnionCase.Name <> newUnionCase.Name
                    then modelTypeChanged.Trigger (this, EventArgs ())
                | false, false ->
                    if oldModelType <> newModelType
                    then modelTypeChanged.Trigger (this, EventArgs ())
                | _ -> modelTypeChanged.Trigger (this, EventArgs ())
            | _ -> ()
        //else ()
  member this.UpdateModel (newModel: 'model) : unit =
    let bindings = this.Bindings
    let propsToNotify =
      bindings
      |> Seq.filter (fun (Kvp (name, binding)) -> this.UpdateValue name newModel binding)
      |> Seq.map Kvp.key
      |> Seq.toList
    let cmdsToNotify =
      bindings
      |> Seq.choose (Kvp.value >> getCmdIfCanExecChanged newModel)
      |> Seq.toList
    this.CurrentModel <- newModel
    propsToNotify |> List.iter notifyPropertyChanged
    cmdsToNotify |> List.iter raiseCanExecuteChanged
    for Kvp (name, binding) in bindings do
      updateValidationError currentModel name binding

  override this.TryGetMember (binder, result) =
    log "[%s] TryGetMember %s" propNameChain binder.Name
    match this.Bindings.TryGetValue binder.Name with
    | false, _ ->
        log "[%s] TryGetMember FAILED: Property %s doesn't exist" propNameChain binder.Name
        false
    | true, binding ->
        result <- tryGetMember currentModel binding
        true

  member internal ___.TryGetMember binding = tryGetMember currentModel binding

  member internal __.CanSetMember = canSetMember

  override this.TrySetMember (binder, value) =
    log "[%s] TrySetMember %s" propNameChain binder.Name
    match this.Bindings.TryGetValue binder.Name with
    | false, _ ->
        log "[%s] TrySetMember FAILED: Property %s doesn't exist" propNameChain binder.Name
        false
    | true, binding ->
        let success = trySetMember currentModel value binding
        if not success then
          log "[%s] TrySetMember FAILED: Binding %s is read-only" propNameChain binder.Name
        success

  member internal __.TrySetMember(value, binding) = trySetMember currentModel value binding


  interface INotifyPropertyChanged with
    [<CLIEvent>]
    member __.PropertyChanged = propertyChanged.Publish

  interface INotifyDataErrorInfo with
    [<CLIEvent>]
    member __.ErrorsChanged = errorsChanged.Publish
    member __.HasErrors =
      errors.Count > 0
    member __.GetErrors propName =
      log "[%s] GetErrors %s" propNameChain (propName |> Option.ofObj |> Option.defaultValue "<null>")
      match errors.TryGetValue propName with
      | true, err -> upcast err
      | false, _ -> null

#if __UWP__

  member private _.GetProperty(name : string) : ICustomProperty =
    if name = "CurrentModel" then DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name, fun vm -> vm.CurrentModel |> box) :> _
    else
    match this.Bindings.TryGetValue name with
    | false, _ ->
      System.Diagnostics.Debugger.Break()
      null
    | true, binding ->
    match binding with
    | OneWay oneWay ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name, fun vm -> vm.TryGetMember(OneWay oneWay)) :> _
    | OneWayLazy oneWayLazy ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name, fun vm -> vm.TryGetMember(OneWayLazy oneWayLazy)) :> _
    | OneWaySeq oneWaySeq ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, ObservableCollection<obj>>(name,
          fun vm -> vm.TryGetMember(OneWaySeq oneWaySeq) :?> _) :> _
    | TwoWay twoWay ->
        let twoWay = TwoWay twoWay
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name,
          (fun vm -> vm.TryGetMember(twoWay)),
          (fun vm value -> vm.TrySetMember(value, twoWay) |> ignore)) :> _
    | TwoWayValidate twoWayValidate ->
        let twoWayValidate = TwoWayValidate twoWayValidate
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name,
          (fun vm -> vm.TryGetMember(twoWayValidate)),
          (fun vm value -> vm.TrySetMember(value, twoWayValidate) |> ignore)) :> _
    | Cmd cmd -> DynamicCustomProperty<ViewModel<'model, 'msg>, System.Windows.Input.ICommand>(name, fun vm -> vm.TryGetMember(Cmd cmd) :?> _) :> _
    | CmdParam cmdParam ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name,
          fun vm -> vm.TryGetMember(CmdParam cmdParam)) :> _
    | SubModel subModel ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, ViewModel<obj, obj>>(name,
          fun vm -> vm.TryGetMember(SubModel subModel) :?> _) :> _
    | SubModelSeq subModelSeq ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, ObservableCollection<ViewModel<obj, obj>>>(name,
          fun vm -> vm.TryGetMember(SubModelSeq subModelSeq) :?> _) :> _
    | SubModelSelectedItem subModelSelectedItem ->
        DynamicCustomProperty<ViewModel<'model, 'msg>, ViewModel<obj, obj>>(name,
          fun vm -> vm.TryGetMember(SubModelSelectedItem subModelSelectedItem) :?> _) :> _
    | Cached cached ->
        let cached = Cached cached
        DynamicCustomProperty<ViewModel<'model, 'msg>, obj>(name,
          (fun vm -> vm.TryGetMember(cached)),
          (fun vm value -> vm.TrySetMember(value, cached) |> ignore)) :> _

  interface ICustomPropertyProvider with

    member this.GetCustomProperty(name) = this.GetProperty(name)

    member this.GetIndexedProperty(name, ``type`` : Type) = this.GetProperty(name)

    member this.GetStringRepresentation() = this.CurrentModel.ToString()

    member this.Type = this.CurrentModel.GetType()
#endif
