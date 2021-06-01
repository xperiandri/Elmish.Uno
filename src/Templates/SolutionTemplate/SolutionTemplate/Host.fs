module SolutionTemplate.Host

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Linq
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Options
open Xamarin.Essentials

open SolutionTemplate.Programs

let configureHost (builder : IConfigurationBuilder) =

    if DeviceInfo.Platform <> DevicePlatform.UWP then
        let root = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "root")
        Directory.CreateDirectory(root) |> ignore
        let inMemoryCollection = seq {
            struct(HostDefaults.ContentRootKey, root)
        }
        builder.AddInMemoryCollection (inMemoryCollection.ToDictionary((fun struct(k,_) -> k), (fun struct(_,v) -> v))) |> ignore

let configureConfiguration (ctx : HostBuilderContext) (builder : IConfigurationBuilder) =

    // TODO: Use some other way usable for conditional builds
    let environmentName = Environments.Development
    ctx.HostingEnvironment.EnvironmentName <- environmentName
    //let environmentName = ctx.HostingEnvironment.EnvironmentName
    //let inMemoryCollection = seq {

    //    //if not (DeviceInfo.Platform = DevicePlatform.UWP) then
    //    //    let root = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "root")
    //    //    Directory.CreateDirectory(root)
    //    //    yield struct(HostDefaults.ContentRootKey, root)

    //    yield!
    //        match environmentName with
    //        | name when name.Equals(Environments.DevelopmentLocal, StringComparison.CurrentCultureIgnoreCase) ->
    //            let endpoint =
    //                if DeviceInfo.Platform = DevicePlatform.Android
    //                    then "http://10.0.2.2:7071/GraphQL"
    //                    else "http://localhost:7071/GraphQL"
    //            seq {
    //                yield struct("GraphQL:Endpoint", endpoint)
    //            }
    //        | name when name.Equals(Environments.Development, StringComparison.CurrentCultureIgnoreCase) ->
    //            seq {
    //                yield struct("GraphQL:Endpoint", "https://host.net/GraphQL")
    //            }
    //        | name when name.Equals("Production", StringComparison.CurrentCultureIgnoreCase) ->
    //            seq {
    //                yield struct("GraphQL:Endpoint", "https://host.net/GraphQL")
    //            }
    //        | _ -> raise <| NotSupportedException ()

    //    yield struct("AzureMaps:Key", BingAutocomplete.key)
    //}

    //builder.AddInMemoryCollection (inMemoryCollection.ToDictionary((fun struct(k,_) -> k), (fun struct(_,v) -> v))) |> ignore
    //()

let configureServices (ctx : HostBuilderContext) (services : IServiceCollection) =
    services
        .AddScoped<Notification.Program>()
        .AddScoped<App.Program>()
    |> ignore

[<CompiledName("CreateDefaultBuilder")>]
let createDefaultBuilder () =

    HostBuilder()
        .ConfigureHostConfiguration(Action<_>(configureHost))
        .ConfigureAppConfiguration(configureConfiguration)
        .ConfigureServices(configureServices)

[<CompiledName("ElmConfig")>]
let elmConfig = { Elmish.Uno.ElmConfig.Default
                  with LogConsole = System.Diagnostics.Debugger.IsAttached }
