namespace SolutionTemplate.Programs.Messages

open Elmish

open SolutionTemplate.Models


type ProgramMessage<'globalMsg, 'localMsg> =
    | Local of 'localMsg
    | Global of 'globalMsg

module ProgramMessage =
    let map mapping cmd =
        cmd
        |> Cmd.map
            (function
            | Global msg -> Global msg
            | Local msg -> msg |> mapping |> Local)

module Cmd =
    let mapLocal mapping (cmd: Cmd<ProgramMessage<'globalMsg, 'localMsg>>) = cmd |> ProgramMessage.map mapping

type RootMsg =
    | Notify of Notification
