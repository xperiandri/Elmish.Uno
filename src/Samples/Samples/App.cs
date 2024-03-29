namespace Elmish.Uno.Samples;

using System.Diagnostics.CodeAnalysis;

public class App : Application
{
    protected internal Window? MainWindow { get; private set; }

    protected override void OnLaunched([NotNull] LaunchActivatedEventArgs args)
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        MainWindow = new Window();
#else
        MainWindow = Microsoft.UI.Xaml.Window.Current;
#endif

#if DEBUG
        MainWindow.EnableHotReload();
#endif


        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Shell shell)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            shell = new Shell();

            // Place the frame in the current Window
            MainWindow.Content = shell;

            shell.RootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (shell.RootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            shell.Navigate(typeof(MainPage), args.Arguments);
        }

        // Ensure the current window is active
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }
}
