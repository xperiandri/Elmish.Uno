using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Elmish.Uno.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly Frame frame;
        private readonly IReadOnlyDictionary<string, Type> pageMap;

        public NavigationService(Frame frame, IReadOnlyDictionary<string, Type> pageMap)
        {
            this.frame = frame;
            this.pageMap = pageMap;
        }

        public NavigationService(Frame frame, IEnumerable<KeyValuePair<string, Type>> pageMap)
        {
            this.frame = frame;
            this.pageMap = pageMap.Aggregate(
                    ImmutableDictionary<string, Type>.Empty.ToBuilder(),
                    (builder, kvp) => { builder.Add(kvp.Key, kvp.Value); return builder; })
                .ToImmutable();
        }

        public int CacheSize => frame.CacheSize;

        public int BackStackDepth => frame.BackStackDepth;

        public bool CanGoBack => frame.CanGoBack;

        public bool CanGoForward => frame.CanGoForward;

        public string GetNavigationState() => frame.GetNavigationState();
        public void SetNavigationState(string navigationState) => frame.SetNavigationState(navigationState);
        public void GoBack() => frame.GoBack();
        public void GoForward() => frame.GoForward();
        public bool Navigate(string name) => frame.Navigate(pageMap[name], null);
        public bool Navigate(string name, IReadOnlyDictionary<string, object> navigationParams) => frame.Navigate(pageMap[name], navigationParams);
    }
}
