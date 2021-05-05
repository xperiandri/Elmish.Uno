namespace SolutionTemplate.Models


open System
open System.Threading.Tasks
open Elmish

type CardCode = string
type CarId = Guid
type CarRId = string
type OrderId = Guid
type ClientId = Guid

type IAppMsg =
    interface
    end

type InfoBarSeverity =
    | Informational = 0
    | Success = 1
    | Warning = 2
    | Error = 3

type Notification =
    { Type: string
      Title: string
      Text: string
      LifeTime: int voption
      IsOpen: bool
      IsClosable: bool }

module Notification =

    let ErrorWithTimer title text lifeTime =
        { Type = "Error"
          Title = title
          Text = text
          LifeTime = ValueSome lifeTime
          IsOpen = true
          IsClosable = true }

    let Error title text =
        { Type = "Error"
          Title = title
          Text = text
          LifeTime = ValueNone
          IsOpen = true
          IsClosable = true }

    let InfoWithTimer title text lifeTime =
        { Type = "Informational"
          Title = title
          Text = text
          LifeTime = ValueSome lifeTime
          IsOpen = true
          IsClosable = true }

    let Info title text =
        { Type = "Informational"
          Title = title
          Text = text
          LifeTime = ValueNone
          IsOpen = true
          IsClosable = true }
