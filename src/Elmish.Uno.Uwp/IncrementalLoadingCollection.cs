using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Elmish.Uno
{
    internal class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {

        private readonly Func<bool> hasMoreItems;
        private readonly Action<uint, TaskCompletionSource<uint>> loadMoreItems;

        public IncrementalLoadingCollection(Func<bool> hasMoreItems, Action<uint, TaskCompletionSource<uint>> loadMoreItems)
        {
            this.hasMoreItems = hasMoreItems;
            this.loadMoreItems = loadMoreItems;
        }

        public IncrementalLoadingCollection(IEnumerable<T> collection, Func<bool> hasMoreItems, Action<uint, TaskCompletionSource<uint>> loadMoreItems) : base(collection)
        {
            this.hasMoreItems = hasMoreItems;
            this.loadMoreItems = loadMoreItems;
        }

        public IncrementalLoadingCollection(List<T> list, Func<bool> hasMoreItems, Action<uint, TaskCompletionSource<uint>> loadMoreItems) : base(list)
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
                loadMoreItems(count, tcs);
                var actualCount = await tcs.Task;
                return new LoadMoreItemsResult { Count = actualCount };
            }
            return LoadMoreItemsAsync().AsAsyncOperation();
        }
    }
}
