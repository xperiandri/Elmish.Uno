namespace Elmish.Uno.Navigation

open System
open System.Collections.Generic
open FSharp.Collections.Immutable
open Windows.UI.Xaml.Controls

type public NavigationService (frame : Frame, pageMap : IReadOnlyDictionary<string, Type>) =

  new (frame, pageMap : KeyValuePair<string, Type> seq) =
    let pageMap' = pageMap
                   |> Seq.fold
                         (fun (b : HashMap<string, Type>.Builder) p -> b.Add (p.Key, p.Value); b)
                         (HashMap<string, Type>.Empty.ToBuilder())
    NavigationService (frame,  pageMap'.ToImmutable())

  interface INavigationService with
    member this.BackStackDepth = frame.BackStackDepth
    member this.CacheSize      = frame.CacheSize
    member this.CanGoBack      = frame.CanGoBack
    member this.CanGoForward   = frame.CanGoForward


    member this.GetNavigationState (): string =
      frame.GetNavigationState ()

    member this.SetNavigationState(navigationState: string): unit =
      frame.SetNavigationState (navigationState)

    member this.GoBack (): unit =
      frame.GoBack ()

    member this.GoForward (): unit =
      frame.GoForward ()

    member this.Navigate (name: string, navigationParams: INavigationParameters): bool =
      frame.Navigate (pageMap.[name], navigationParams)

    member this.Navigate (name: string): bool =
      frame.Navigate (pageMap.[name], null)
