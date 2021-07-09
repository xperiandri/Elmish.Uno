using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using AppProgram = SolutionTemplate.Programs.App.Program;

//-:cnd:noEmit
namespace SolutionTemplate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;
#pragma warning disable IDE0044 // Add readonly modifier
        private IServiceScope scope;
#pragma warning restore IDE0044 // Add readonly modifier
        private readonly Lazy<Shell> shell = new Lazy<Shell>();

#if NET5_0 && WINDOWS
        private Window window;

#else
        private Windows.UI.Xaml.Window window;
#endif

        internal IServiceProvider ServiceProvider => scope.ServiceProvider;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();
            this.InitializeComponent();

            var hostBuilder = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices);
            serviceProvider = hostBuilder.Build().Services;
            scope = serviceProvider.CreateScope();

#if HAS_UNO || NETFX_CORE
            this.Suspending += OnSuspending;
#endif
        }

        private void ConfigureServices(HostBuilderContext ctx, IServiceCollection services) =>
            services
            .AddSingleton<Elmish.Uno.Navigation.INavigationService>(provider =>
                new Elmish.Uno.Navigation.NavigationService(
                    shell.Value.RootFrame,
                    new Dictionary<string, Type>()
                    {
                        [Pages.Pages.Main] = typeof(MainPage)
                    }))
            ;

#pragma warning disable IDE0051 // Remove unused private members
        private void ResetScope()
        {
            scope.Dispose();
            scope = serviceProvider.CreateScope();
            var viewModel = ServiceProvider.GetRequiredService<AppProgram>();
            Elmish.Uno.ViewModel.StartLoop(Host.ElmConfig, shell.Value, Elmish.ProgramModule.run, viewModel.Program);
        }
#pragma warning restore IDE0051 // Remove unused private members

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1725 // Parameter names should match base declaration
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

#if NET5_0 && WINDOWS
            window = new Window();
            window.Activate();
#else
            window = Windows.UI.Xaml.Window.Current;
#endif
            var shell = this.shell.Value;
            // Get a Frame to act as the navigation context and navigate to the first page
            var rootFrame = shell.RootFrame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame.Content == null)
            {
                var viewModel = ServiceProvider.GetRequiredService<AppProgram>();
                Elmish.Uno.ViewModel.StartLoop(Host.ElmConfig, shell, Elmish.ProgramModule.run, viewModel.Program);

#pragma warning disable CA1062 // Validate arguments of public methods
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
#pragma warning restore CA1062 // Validate arguments of public methods
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                window.Content = shell;
            }

#if !(NET5_0 && WINDOWS)
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
#pragma warning restore CA1062 // Validate arguments of public methods
#pragma warning restore CA1725 // Parameter names should match base declaration

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
        }
    }
}
//+:cnd:noEmit
