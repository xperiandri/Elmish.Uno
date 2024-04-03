namespace SolutionTemplate;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Uno.Extensions;
using Uno.Extensions.Configuration;
using Uno.Extensions.Hosting;
using Uno.Extensions.Localization;
using Uno.Extensions.Logging;

using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;

using AppProgram = SolutionTemplate.Programs.App.Program;

public sealed partial class App : Application
{
    private IHost CreateHost() =>
        UnoHost
            .CreateDefaultBuilder()
#if DEBUG
            // Switch to Development environment when running in DEBUG
            .UseEnvironment(Environments.Development)
#endif
            // Add platform specific log providers
            .UseLogging((HostBuilderContext context, ILoggingBuilder logBuilder) =>
            {
                var host = context.HostingEnvironment;
                // Configure log levels for different categories of logging
                logBuilder
                        .SetMinimumLevel(host.IsDevelopment() ? LogLevel.Trace : LogLevel.Information)
                        .XamlLogLevel(LogLevel.Information)
                        .XamlLayoutLogLevel(LogLevel.Information);
            })

            .UseLocalization()

            .UseConfiguration(configure: configBuilder =>
                configBuilder
                    .EmbeddedSource<AppProgram>(includeEnvironmentSettings: true)    // appsettings.json + appsettings.development.json
                    .EmbeddedSource<App>("platform") // appsettings.platform.json
            )

            // Register Json serializer jsontypeinfo definitions
            //.UseSerialization(
            //    services => services
            //                    .AddJsonTypeInfo(WidgetContext.Default.Widget)
            //                    .AddJsonTypeInfo(PersonContext.Default.Person)
            //)

            // Register services for the application
            .ConfigureServices(SolutionTemplate.ConfigurationModule.ConfigureCommonServices)
            .ConfigureServices((context, services) =>
            {
                services
                    //.AddNativeHandler()
                    .AddTransient<DebugHttpHandler>()
                    //.AddContentSerializer()
                    .AddSingleton<global::Elmish.Uno.Navigation.INavigationService>(_ =>
                        new global::Elmish.Uno.Navigation.NavigationService(
                            shell.Value.RootFrame,
                            new Dictionary<string, Type>()
                            {
                                [nameof(Pages.Main)] = typeof(MainPage),
                            }));

            })


            // Add navigation support for toolkit controls such as TabBar and NavigationView
            //.UseToolkitNavigation()

            .Build(enableUnoLogging: true);

#pragma warning disable IDE0022 // Use expression body for methods
    private static void LogWebAuthenticationBrokerSettings(ILogger logger)
    {
#if __WASM__
        WinRTFeatureConfiguration.WebAuthenticationBroker.DefaultCallbackPath = "/authentication/login-callback.htm";
#elif !WINDOWS_UWP
        WinRTFeatureConfiguration.WebAuthenticationBroker.DefaultReturnUri = new Uri($"{Constants.AppScheme}://{Constants.AppHost}");
#endif
        logger.LogInformation("WebAuthenticationBroker CallbackUri = '{uri}'", WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
    }
#pragma warning restore IDE0022 // Use expression body for methods

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    private static ILoggingBuilder CreateLoggerFactory(ILoggingBuilder builder)
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
        return builder;
    }
}

public class DebugHttpHandler : DelegatingHandler
{
    public DebugHttpHandler(HttpMessageHandler? innerHandler = null)
        : base(innerHandler ?? new HttpClientHandler())
    {
    }

    protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken);
    }
}
