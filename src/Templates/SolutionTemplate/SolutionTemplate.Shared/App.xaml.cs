//-:cnd:noEmit
namespace SolutionTemplate;

using System;
using System.Diagnostics.Contracts;

using SolutionTemplate.Elmish;

using global::Windows.ApplicationModel;
using global::Windows.ApplicationModel.Activation;
using global::Windows.Networking.Connectivity;
using global::Windows.UI.Xaml;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Uno.Extensions;

using AppProgram = SolutionTemplate.Programs.App.Program;


/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    private readonly IHost host;
    private readonly AppProgram appProgram;
    private readonly Lazy<Shell> shell = new Lazy<Shell>();

#if NET6_0 && WINDOWS
    private Window window;

#else
    private global::Windows.UI.Xaml.Window window;
#endif

    internal IServiceProvider ServiceProvider => host.Services;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        host = CreateHost();
        appProgram = new AppProgram(ServiceProvider);
        LogWebAuthenticationBrokerSettings(host.Log());
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
#pragma warning disable CA1725 // Parameter names should match base declaration
    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        Contract.Assume(e != null, nameof(e) + " is null.");
#if DEBUG
        if (System.Diagnostics.Debugger.IsAttached)
        {
            // this.DebugSettings.EnableFrameRateCounter = true;
        }
#endif

#if NET6_0 && WINDOWS
        window = new Window();
        window.Activate();
#else
        window = global::Windows.UI.Xaml.Window.Current;
#endif
        var shell = this.shell.Value;
        // Get a Frame to act as the navigation context and navigate to the first page
        var rootFrame = shell.RootFrame;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (rootFrame.Content == null)
        {
            var program =
                appProgram.Program
                .WithSubscription(AppProgram.GetLifecycleEventsSubscription(SubscribeToLifecycleEvents))
                .WithSubscription(AppProgram.GetNetworkStatusSubscription(SubscribeToNetworkStatus));

            global::Elmish.Uno.ViewModel.StartLoop(
                ConfigurationModule.ElmConfig, shell, global::Elmish.ProgramModule.runWith, program,
                (SolutionTemplate.WinRT.ApplicationExecutionState)e.PreviousExecutionState);

#pragma warning disable CA1062 // Validate arguments of public methods
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
#pragma warning restore CA1062 // Validate arguments of public methods
            {
                //TODO: Load state from previously suspended application
            }

            // Place the frame in the current Window
            window.Content = shell;
        }

#if !(NET6_0 && WINDOWS)
        if (!e.PrelaunchActivated)
#endif
        {
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            window.Activate();
        }
    }
#pragma warning restore CA1725 // Parameter names should match base declaration

#pragma warning disable RCS1163 // Unused parameter.
    private void SubscribeToLifecycleEvents(
        Action<Exception, string, bool, Action<bool>> onUnhandledException,
        Action<DateTimeOffset, Action> onSuspending,
        Action<object> onResuming,
        Action<Action> onEnteredBackground,
        Action<Action> onLeavingBackground)
    {
        this.UnhandledException +=
            (_, e) => onUnhandledException(e.Exception, e.Message, e.Handled, isHandled => e.Handled = isHandled);

#if HAS_UNO || NETFX_CORE
        // <summary>
        // Invoked when application execution is being suspended.  Application state is saved
        // without knowing whether the application will be terminated or resumed with the contents
        // of memory still intact.
        // </summary>
        // <param name="sender">The source of the suspend request.</param>
        // <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var op = e.SuspendingOperation;
            var deferral = op.GetDeferral();
            onSuspending(op.Deadline, () => deferral.Complete());
        }
        this.Suspending += OnSuspending;
        this.Resuming += (_, o) => onResuming(o);

        this.EnteredBackground += (_, e) => onEnteredBackground(
#if WINDOWS_UWP
            e.GetDeferral().Complete
#else
            () => { }
#endif
        );
        this.LeavingBackground += (_, e) => onLeavingBackground(
#if WINDOWS_UWP
            e.GetDeferral().Complete
#else
            () => { }
#endif
        );
#endif
    }
#pragma warning restore RCS1163 // Unused parameter.
    private static void SubscribeToNetworkStatus(Action<SolutionTemplate.WinRT.NetworkConnectivityLevel> onNetworkChanged)
     => NetworkInformation.NetworkStatusChanged += (sender) =>
     {
         var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
         if (connectionProfile != null)
         {
             var connectivityLevel = connectionProfile.GetNetworkConnectivityLevel();
             onNetworkChanged((SolutionTemplate.WinRT.NetworkConnectivityLevel)connectivityLevel);
         }
         else
         {
             onNetworkChanged(SolutionTemplate.WinRT.NetworkConnectivityLevel.None);
         }
     };
}
//+:cnd:noEmit
