namespace Elmish.Uno

open System
open System.Collections.ObjectModel
open System.Threading.Tasks
open Windows.UI.Xaml.Data

type IncrementalLoadingCollection<'t> =
  inherit ObservableCollection<'t>

  val has: unit -> bool
  val load: uint * (uint -> unit) -> unit

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
      this.load (count, fun count -> tcs.SetResult(count))
      let mapToResult (actualCountTask: Task<uint>) =
        LoadMoreItemsResult(Count = actualCountTask.Result)
      tcs.Task.ContinueWith(mapToResult, TaskContinuationOptions.OnlyOnRanToCompletion).AsAsyncOperation()

      //(async {
      //  let tcs = TaskCompletionSource<uint>()
      //  let count =
      //    try
      //      let! count = this.load (count, fun count -> tcs.SetResult(count))
      //      count
      //    with
      //    | :? Exception as ex ->
      //      tcs.SetException(ex)
      //      ex.Reraise ()
      //  let mapToResult (actualCountTask: Task<uint>) =
      //    LoadMoreItemsResult(Count = actualCountTask.Result)
      //  tcs.Task.ContinueWith(mapToResult, TaskContinuationOptions.OnlyOnRanToCompletion).AsAsyncOperation()
      //  let! count = load count
      //  this.load (count, tcs)
      //  let complete (actualCount: uint) = async {
      //    return LoadMoreItemsResult(Count = actualCountTask.Result)
      //  }
      //} |> Async.StartAsTask()).ContinueWith(mapToResult, TaskContinuationOptions.OnlyOnRanToCompletion).AsAsyncOperation()

