namespace Elmish.Uno

open System
open System.Collections.ObjectModel
open System.Threading.Tasks
open Windows.UI.Xaml.Data

type IncrementalLoadingCollection<'t> =
  inherit ObservableCollection<'t>

  val has: HasMoreItems
  val load: uint * TaskCompletionSource<uint> -> unit

  new (hasMoreItems, loadMoreItems) =
    { inherit ObservableCollection<'t>();
      has = hasMoreItems;
      load = loadMoreItems }

  new (collection: 't seq, hasMoreItems, loadMoreItems)=
    { inherit ObservableCollection<'t>(collection);
      has = hasMoreItems;
      load = loadMoreItems  }

  new (list: 't ResizeArray, hasMoreItems, loadMoreItems) =
    { inherit ObservableCollection<'t>(list);
      has = hasMoreItems;
      load = loadMoreItems  }

  interface ISupportIncrementalLoading with

    member this.HasMoreItems = this.has()

    member this.LoadMoreItemsAsync (count) =
      let tcs = TaskCompletionSource<uint>()
      this.load (count, tcs)
      let mapToResult (actualCountTask: Task<uint>) =
        LoadMoreItemsResult(Count = actualCountTask.Result)
      tcs.Task.ContinueWith(mapToResult, TaskContinuationOptions.OnlyOnRanToCompletion).AsAsyncOperation()

