[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module SolutionTemplate.Options

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection

[<CLIMutable>]
type GraphQLOptions =
    { EndPoint : string }

type IServiceCollection with
    member services.ConfigureCommonOptions (configuration : IConfiguration) =
        services
            .Configure<GraphQLOptions>(configuration.GetSection("GraphQL"))
