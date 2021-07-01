module SolutionTemplate.Host

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Linq
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Xamarin.Essentials

open SolutionTemplate.Configuration
open SolutionTemplate.Programs

let configureHost (builder : IConfigurationBuilder) =

    if DeviceInfo.Platform <> DevicePlatform.UWP then
        let root = Path.Combine (System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "root")
        Directory.CreateDirectory root |> ignore
        let inMemoryCollection = seq {
            struct(HostDefaults.ContentRootKey, root)
        }
        builder.AddInMemoryCollection (inMemoryCollection.ToDictionary(fstv, sndv)) |> ignore

let configureServices (ctx : HostBuilderContext) (services : IServiceCollection) =
    services
        .ConfigureSolutionTemplateOptions(ctx.Configuration)
        .AddScoped<Notification.Program>()
        .AddScoped<App.Program>()
    |> ignore

[<CompiledName("CreateDefaultBuilder")>]
let createDefaultBuilder () =

    let environmentName =
    #if PRODUCTION
        Environments.Production
    #elif STAGING
        Environments.Staging
    #elif LOCALAPI
        Environments.LocalAPI
    #else
        Environments.Development
    #endif

    HostBuilder()
        .UseEnvironment(environmentName)
        .ConfigureHostConfiguration(Action<_>(configureHost))
        .ConfigureAppConfiguration(Configuration.configure)
        .ConfigureServices(configureServices)

[<CompiledName("ElmConfig")>]
let elmConfig = { Elmish.Uno.ElmConfig.Default
                  with LogConsole = System.Diagnostics.Debugger.IsAttached }
