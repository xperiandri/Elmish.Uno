namespace SolutionTemplate.Programs.Messages

open Elmish

open SolutionTemplate.Models

type ProgramMessage<'globalMsg, 'localMsg> =
    | Local of 'localMsg
    | Global of 'globalMsg

module ProgramMessage =
    let map mapping msg =
        match msg with
        | Global msg -> Global msg
        | Local msg -> msg |> mapping |> Local

module Cmd =
    let mapLocal mapping (cmd : Cmd<ProgramMessage<'globalMsg, 'localMsg>>) =
        cmd |> Cmd.map (ProgramMessage.map mapping)

    let ofLocal subModelMsg appMsg value =
        value
        |> subModelMsg
        |> appMsg
        |> Local
        |> Cmd.ofMsg

type RootMsg =
    | Notify of Notification
