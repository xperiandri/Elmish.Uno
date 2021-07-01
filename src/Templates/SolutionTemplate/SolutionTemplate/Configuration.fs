[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module SolutionTemplate.Configuration

open System
open System.Linq
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Xamarin.Essentials

module Environments =
    let LocalAPI = "LocalAPI"

let configure (ctx : HostBuilderContext) (builder : IConfigurationBuilder) =

    let environmentName = ctx.HostingEnvironment.EnvironmentName

    let inMemoryCollection = seq {

        yield!
            match environmentName with
            | name when name.Equals(Environments.LocalAPI, StringComparison.CurrentCultureIgnoreCase) ->
                let endpoint =
                    if DeviceInfo.Platform = DevicePlatform.Android
                    then "http://10.0.2.2:7071/GraphQL"
                    else "http://localhost:7071/GraphQL"
                seq {
                    yield struct("GraphQL:EndPoint", endpoint)
                }
            | name when name.Equals(Environments.Development, StringComparison.CurrentCultureIgnoreCase) ->
                seq {
                    yield struct("GraphQL:EndPoint", "https://host.net/GraphQL")
                }
            | name when name.Equals(Environments.Production, StringComparison.CurrentCultureIgnoreCase) ->
                seq {
                    yield struct("GraphQL:EndPoint", "https://host.net/GraphQL")
                }
            | _ -> raise <| NotSupportedException ()

        //yield struct("AzureMaps:Key", BingAutocomplete.key)
    }

    builder.AddInMemoryCollection (inMemoryCollection.ToDictionary(fstv, sndv)) |> ignore
    ()

[<CLIMutable>]
type GraphQLSettings =
    { EndPoint : string }

open Microsoft.Extensions.DependencyInjection

type IServiceCollection with
    member services.ConfigureSolutionTemplateOptions (configuration : IConfiguration) =
        services.Configure<GraphQLSettings>(configuration.GetSection("GraphQL"))
