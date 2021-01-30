namespace Elmish.Uno.Navigation

open System
open System.Collections.Generic

type public NavigationParameter   = KeyValuePair<string, obj>
type public INavigationParameters = IReadOnlyDictionary<string, obj>

type public INavigationService =
  abstract Navigate           : name : string                                              -> bool
  abstract Navigate           : name : string *  navigationParams : INavigationParameters  -> bool

  abstract GoBack             : unit                                                       -> unit
  abstract GoForward          : unit                                                       -> unit
  abstract GetNavigationState : unit                                                       -> string
  abstract SetNavigationState : navigationState : string                                   -> unit

  abstract CacheSize          : int
  abstract BackStackDepth     : int
  abstract CanGoBack          : bool
  abstract CanGoForward       : bool
