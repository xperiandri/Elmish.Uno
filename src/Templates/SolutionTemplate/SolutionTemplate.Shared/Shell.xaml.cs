using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using System.Timers;
using Microsoft.Toolkit.Uwp.Helpers;

namespace SolutionTemplate
{
    public abstract partial class ShellBase : UserControl
    {
        #region NavigationalErrorCommand

        /// <summary>
        /// NavigateError Dependency Property
        /// </summary>
        public static readonly DependencyProperty NavigationalErrorCommandProperty =
            DependencyProperty.Register(nameof(NavigationalErrorCommand), typeof(ICommand), typeof(ShellBase),
                new PropertyMetadata((ICommand)null));

        /// <summary>
        /// Gets or sets the NavigateError property. This dependency property
        /// indicates ....
        /// </summary>
        public ICommand NavigationalErrorCommand
        {
            get => (ICommand)GetValue(NavigationalErrorCommandProperty);
            set => SetValue(NavigationalErrorCommandProperty, value);
        }

        #endregion
    }
    public partial class Shell : ShellBase, INavigate
    {
        public Frame RootFrame => this.rootFrame;

        //public InAppNotification Notification => this.notification;
        private System.Timers.Timer timer;
        public Shell()
        {
            this.InitializeComponent();
            //this.notification.AnimationDuration = TimeSpan.FromMilliseconds(100);
            RootFrame.Navigated += RootFrame_Navigated;
            DataContextChanged += OnDataContextChanged;
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (RootFrame.Content is Control control)
            {
                //var expression = control.GetBindingExpression(Control.DataContextProperty);
                //var binding = new Binding{ Path = expression.ParentBinding.Path, Source = DataContext };
                ////binding.Source = DataContext;

                //control.SetBinding(DataContextProperty, binding);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {

            //this.notification.Show($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
            //Notifications.Add(new NotificationData("Error", $"Failed to load {e.SourcePageType.FullName}: {e.Exception}","Error"));
            NavigationalErrorCommand?.Execute($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");

        }
        private void OnDataContextChanged(object sender, DataContextChangedEventArgs e)
        {
            //Root.DataContext = null;
            //RootFrame.DataContext = e.NewValue;
            //Root.DataContext = DependencyProperty.UnsetValue;
            //Root.SetBinding(DataContextProperty, new Binding() { Source = e.NewValue });
        }
        public bool Navigate(Type sourcePageType) => this.rootFrame.Navigate(sourcePageType, null);

        public bool Navigate(Type sourcePageType, object parameter) => this.rootFrame.Navigate(sourcePageType, parameter);

        //public void Dismiss() => this.notification.Dismiss();
    }
}
