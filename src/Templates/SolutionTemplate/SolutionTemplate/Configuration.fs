[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module SolutionTemplate.Configuration

open System
open System.Linq
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Xamarin.Essentials

module Environments =
    let LocalAPI = "LocalAPI"

[<CompiledName "Configure">]
let configure (ctx : HostBuilderContext) (builder : IConfigurationBuilder) =

    let inMemoryCollection = seq {

        //yield!
        //    match ctx.HostingEnvironment.EnvironmentName with
        //    | name when name.Equals(Environments.LocalAPI, StringComparison.CurrentCultureIgnoreCase) ->
        //        seq {
        //            yield struct("GraphQL:EndPoint", "https://host.net/GraphQL")
        //        }
        //    | name when name.Equals(Environments.Development, StringComparison.CurrentCultureIgnoreCase) ->
        //        seq {
        //            yield struct("GraphQL:EndPoint", "https://host.net/GraphQL")
        //        }
        //    | name when name.Equals(Environments.Production, StringComparison.CurrentCultureIgnoreCase) ->
        //        seq {
        //            yield struct("GraphQL:EndPoint", "https://host.net/GraphQL")
        //        }
        //    | _ -> raise <| NotSupportedException ()

        yield struct("GraphQL:EndPoint", Constants.GraphQLEndPoint)
    }

    builder.AddInMemoryCollection (inMemoryCollection.ToDictionary(fstv, sndv)) |> ignore
    ()

[<CLIMutable>]
type GraphQLOptions =
    { EndPoint : string }

open Microsoft.Extensions.DependencyInjection

type IServiceCollection with
    member services.ConfigureSolutionTemplateOptions (configuration : IConfiguration) =
        services
            .Configure<GraphQLOptions>(configuration.GetSection("GraphQL"))

