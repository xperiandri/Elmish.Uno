[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module SolutionTemplate.Configuration

open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open SolutionTemplate.Options
open SolutionTemplate.Programs

[<CompiledName("ElmConfig")>]
let elmConfig = { Elmish.Uno.ElmConfig.Default
                    with LogConsole = System.Diagnostics.Debugger.IsAttached }

[<CompiledName "ConfigureCommonServices">]
let configureCommonServices (ctx : HostBuilderContext) (services : IServiceCollection) =
    services
        .ConfigureCommonOptions(ctx.Configuration)
        .AddScoped<Notifications.Program>()
        .AddScoped<App.Program>()
    |> ignore
