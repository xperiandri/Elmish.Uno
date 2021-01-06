module Elmish.Uno.Samples.SubModel.Program

open System
open Elmish
open Elmish.Uno

module Counter = Elmish.Uno.Samples.SingleCounter.Program

module Clock =

  type Model =
    { Time: DateTimeOffset
      UseUtc: bool }

  let initial =
    { Time = DateTimeOffset.Now
      UseUtc = false }

  let init () = initial

  let getTime m =
    if m.UseUtc then m.Time.UtcDateTime else m.Time.LocalDateTime

  type Msg =
    | Tick of DateTimeOffset
    | ToggleUtc

  let update msg m =
    match msg with
    | Tick t -> { m with Time = t }
    | ToggleUtc -> { m with UseUtc = not m.UseUtc }

  [<CompiledName("Bindings")>]
  let bindings () : Binding<Model, Msg> list = [
    "Time" |> Binding.oneWay getTime
    "ToggleUtc" |> Binding.cmd ToggleUtc
  ]

  [<CompiledName("DesignModel")>]
  let designModel = initial


module CounterWithClock =

  type Model =
    { Counter: Counter.Model
      Clock: Clock.Model }

  let initial =
    { Counter = Counter.initial
      Clock = Clock.init () }

  let init () = initial

  type Msg =
    | CounterMsg of Counter.Msg
    | ClockMsg of Clock.Msg

  let update msg m =
    match msg with
    | CounterMsg msg -> { m with Counter = Counter.update msg m.Counter }
    | ClockMsg msg -> { m with Clock = Clock.update msg m.Clock }

  [<CompiledName("Bindings")>]
  let bindings () : Binding<Model, Msg> list = [
    "Counter" |> Binding.subModel((fun m -> m.Counter), snd, CounterMsg, fun () -> Counter.bindings)
    "Clock" |> Binding.subModel((fun m -> m.Clock), snd, ClockMsg, Clock.bindings)
  ]

  [<CompiledName("DesignModel")>]
  let designModel = initial

module App =

  type Model =
    { ClockCounter1: CounterWithClock.Model
      ClockCounter2: CounterWithClock.Model }

  let init () =
    { ClockCounter1 = CounterWithClock.init ()
      ClockCounter2 = CounterWithClock.init () }

  type Msg =
    | ClockCounter1Msg of CounterWithClock.Msg
    | ClockCounter2Msg of CounterWithClock.Msg

  let update msg m =
    match msg with
    | ClockCounter1Msg msg ->
        { m with ClockCounter1 = CounterWithClock.update msg m.ClockCounter1 }
    | ClockCounter2Msg msg ->
        { m with ClockCounter2 = CounterWithClock.update msg m.ClockCounter2 }

  let bindings : Binding<Model, Msg> list = [
    "ClockCounter1" |> Binding.subModel(
      (fun m -> m.ClockCounter1),
      snd,
      ClockCounter1Msg,
      CounterWithClock.bindings)

    "ClockCounter2" |> Binding.subModel(
      (fun m -> m.ClockCounter2),
      snd,
      ClockCounter2Msg,
      CounterWithClock.bindings)
  ]

let timerTick dispatch =
  let timer = new System.Timers.Timer(1000.)
  timer.Elapsed.Add (fun _ ->
    let clockMsg =
      DateTimeOffset.Now
      |> Clock.Tick
      |> CounterWithClock.ClockMsg
    dispatch <| App.ClockCounter1Msg clockMsg
    dispatch <| App.ClockCounter2Msg clockMsg
  )
  timer.Start()


[<CompiledName("DesignModel")>]
let designModel : App.Model =
  { ClockCounter1 = CounterWithClock.initial
    ClockCounter2 = CounterWithClock.initial }

[<CompiledName("Program")>]
let program =
  Program.mkSimpleUno App.init App.update App.bindings
  |> Program.withSubscription (fun m -> Cmd.ofSub timerTick)
  |> Program.withConsoleTrace

[<CompiledName("Config")>]
let config = { ElmConfig.Default with LogConsole = true }
