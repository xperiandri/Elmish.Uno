using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Microsoft.FSharp.Core;

using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Elmish.Uno
{
    internal class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {

        private readonly Func<bool> hasMoreItems;
        private readonly Action<uint, FSharpFunc<uint, Unit>> loadMoreItems;

        public IncrementalLoadingCollection(Func<bool> hasMoreItems, Action<uint, FSharpFunc<uint, Unit>> loadMoreItems)
        {
            this.hasMoreItems = hasMoreItems;
            this.loadMoreItems = loadMoreItems;
        }

        public IncrementalLoadingCollection(IEnumerable<T> collection, Func<bool> hasMoreItems, Action<uint, FSharpFunc<uint, Unit>> loadMoreItems) : base(collection)
        {
            this.hasMoreItems = hasMoreItems;
            this.loadMoreItems = loadMoreItems;
        }

        public IncrementalLoadingCollection(List<T> list, Func<bool> hasMoreItems, Action<uint, FSharpFunc<uint, Unit>> loadMoreItems) : base(list)
        {
            this.hasMoreItems = hasMoreItems;
            this.loadMoreItems = loadMoreItems;
        }

        public bool HasMoreItems => hasMoreItems();

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            async Task<LoadMoreItemsResult> LoadMoreItemsAsync ()
            {
                var tcs = new TaskCompletionSource<uint>();
                loadMoreItems(count, FuncConvert.FromAction<uint>(c => tcs.SetResult(c)));
                var actualCount = await tcs.Task;
                return new LoadMoreItemsResult { Count = actualCount };
            }
            return LoadMoreItemsAsync().AsAsyncOperation();
        }
    }
}
