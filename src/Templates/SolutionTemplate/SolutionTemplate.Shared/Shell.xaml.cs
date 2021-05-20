using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SolutionTemplate.Pages;

namespace SolutionTemplate
{
#pragma warning disable CA1010 // Generic interface should also be implemented
    public abstract partial class ShellBase : UserControl
#pragma warning restore CA1010 // Generic interface should also be implemented
    {
        #region NavigationFailedCommand

        /// <summary>
        /// NavigationFailedCommand Dependency Property
        /// </summary>
        public static readonly DependencyProperty NavigationFailedCommandProperty =
            DependencyProperty.Register(nameof(NavigationFailedCommand), typeof(ICommand), typeof(ShellBase),
                new PropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets or sets the NavigationFailedCommand property. This dependency property
        /// contains a command that is invoked when navigation fails.
        /// </summary>
        public ICommand NavigationFailedCommand
        {
            get => (ICommand)GetValue(NavigationFailedCommandProperty);
            set => SetValue(NavigationFailedCommandProperty, value);
        }

        #endregion
    }
#pragma warning disable CA1724
#pragma warning disable CA1501
#pragma warning disable CA1010 // Generic interface should also be implemented
    public partial class Shell : ShellBase, INavigate
#pragma warning restore CA1010 // Generic interface should also be implemented
#pragma warning disable CA1501
#pragma warning disable CA1724
    {
        public Shell()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {

            var error = new NavigationError(() => e.Handled, h => e.Handled = h, e.Exception, e.SourcePageType);
            NavigationFailedCommand?.Execute(error);
        }

        public bool Navigate(Type sourcePageType) => this.RootFrame.Navigate(sourcePageType, null);

        public bool Navigate(Type sourcePageType, object parameter) => this.RootFrame.Navigate(sourcePageType, parameter);
    }
}
