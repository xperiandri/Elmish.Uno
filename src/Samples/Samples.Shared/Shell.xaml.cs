using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Elmish.Uno.Samples
{
    public sealed partial class Shell : UserControl, INavigate
    {

        public Frame RootFrame => this.rootFrame;

        public Shell()
        {
            this.InitializeComponent();

            SystemNavigationManager.GetForCurrentView().BackRequested += OnSystemNavigationManagerBackRequested;
            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
        }

        private bool OnBackRequested()
        {
            if (this.rootFrame.CanGoBack)
            {
                this.rootFrame.GoBack();
                return true;
            }
            return false;
        }

        private void OnSystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
        {
            OnBackRequested();
            e.Handled = true;
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e) => OnBackRequested();

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            OnBackRequested();
            e.Handled = true;
        }

        public bool Navigate(Type sourcePageType) => this.rootFrame.Navigate(sourcePageType, null);

        public bool Navigate(Type sourcePageType, object parameter) => this.rootFrame.Navigate(sourcePageType, parameter);
    }
}
