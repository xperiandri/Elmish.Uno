namespace SolutionTemplate;

using System;
using System.Windows.Input;

using global::Windows.UI.Xaml;
using global::Windows.UI.Xaml.Controls;
using global::Windows.UI.Xaml.Navigation;

public abstract partial class ShellBase : UserControl
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

public partial class Shell : ShellBase, INavigate
{
    public Shell()
    {
        this.InitializeComponent();
    }

#pragma warning disable RCS1163 // Unused parameter.
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
#pragma warning restore RCS1163 // Unused parameter.

    public bool Navigate(Type sourcePageType) => this.RootFrame.Navigate(sourcePageType, null);

    public bool Navigate(Type sourcePageType, object parameter) => this.RootFrame.Navigate(sourcePageType, parameter);
}
