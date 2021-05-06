using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;

using SolutionTemplate.Models;
using SolutionTemplate.Controls;

using SolutionTemplate.Pages;
using Elmish.Uno;
using Elmish.Uno.Navigation;

using FSharpx;
using AppProgram = SolutionTemplate.Programs.App.Program;

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

        private global::Windows.UI.Xaml.Window window;

        internal IServiceProvider ServiceProvider => scope.ServiceProvider;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeLogging();
            var hostBuilder =
                Host.CreateDefaultBuilder()
                    .ConfigureServices(ConfigureServices)
                    ;

#pragma warning disable DF0021 // Marks indisposed objects assigned to a field, originated from method invocation.
#pragma warning disable DF0025 // Marks indisposed objects assigned to a field, originated from method invocation.
            serviceProvider = hostBuilder.Build().Services;
            scope = serviceProvider.CreateScope();
#pragma warning restore DF0021 // Marks indisposed objects assigned to a field, originated from method invocation.
#pragma warning restore DF0025 // Marks indisposed objects assigned to a field, originated from method invocation.
            this.InitializeComponent();

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

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            window = global::Windows.UI.Xaml.Window.Current;

            var shell = this.shell.Value;
            var rootFrame = shell.RootFrame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame.Content == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                shell = new Shell();
                var viewModel = ServiceProvider.GetRequiredService<AppProgram>();
                Elmish.Uno.ViewModel.StartLoop(Host.ElmConfig, shell, Elmish.ProgramModule.run, viewModel.Program);
                // Place the frame in the current Window
                Windows.UI.Xaml.Window.Current.Content = shell;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                window.Content = shell;
            }

            if (e.PrelaunchActivated == false)
            {
                if (shell.RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    shell.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                window.Activate();
            }
        }

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
        /// Configures global logging
        /// </summary>
        private static void InitializeLogging()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();

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
