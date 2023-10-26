namespace Elmish.Uno.Samples;

using System;

using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

    public sealed partial class Shell : UserControl, INavigate
    {
        public object PageTypeName => RootFrame.Content?.GetType().Name;

        public Shell()
        {
            this.InitializeComponent();

#if !(NET6_0 && WINDOWS)
            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemNavigationManagerBackRequested;
#endif

#pragma warning disable Uno0001 // Uno type or member is not implemented
        KeyboardAccelerator GoBack = new KeyboardAccelerator()
            {
                Key = VirtualKey.GoBack
            };
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator()
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);

            KeyboardAccelerator GoForward = new KeyboardAccelerator()
            {
                Key = VirtualKey.GoForward
            };
            GoForward.Invoked += ForwardInvoked;
            KeyboardAccelerator AltRight = new KeyboardAccelerator()
            {
                Key = VirtualKey.Right,
                Modifiers = VirtualKeyModifiers.Menu
            };
            AltRight.Invoked += ForwardInvoked;
            this.KeyboardAccelerators.Add(GoForward);
            this.KeyboardAccelerators.Add(AltRight);
#pragma warning restore Uno0001 // Uno type or member is not implemented
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
#pragma warning disable CA2201 // Do not raise reserved exception types
         => throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
#pragma warning restore CA2201 // Do not raise reserved exception types

#pragma warning restore CA2201

    private bool OnBackRequested()
    {
        if (this.RootFrame.CanGoBack)
        {
            this.RootFrame.GoBack();
            return true;
        }
        return false;
    }

    private bool OnForwardRequested()
    {
        if (this.RootFrame.CanGoForward)
        {
            this.RootFrame.GoForward();
            return true;
        }
        return false;
    }

#if !(NET6_0 && WINDOWS)
    private void OnSystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
    {
        OnBackRequested();
        e.Handled = true;
    }
#endif

    private void OnBackButtonClick(object sender, RoutedEventArgs e) => OnBackRequested();

#pragma warning disable Uno0001 // Uno type or member is not implemented
        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            OnBackRequested();
            e.Handled = true;
        }

        private void ForwardInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            OnForwardRequested();
            e.Handled = true;
        }
#pragma warning restore Uno0001 // Uno type or member is not implemented

    public bool Navigate(Type sourcePageType) => this.RootFrame.Navigate(sourcePageType, null);

    public bool Navigate(Type sourcePageType, object parameter) => this.RootFrame.Navigate(sourcePageType, parameter);
}
