namespace SolutionTemplate

open Xamarin.Essentials

#if !PRODUCTION
module Constants =

#if LOCALAPI
    let GraphQLEndPoint =
        if DeviceInfo.Platform = DevicePlatform.Android
        then "http://10.0.2.2:7071/GraphQL"
        else "http://localhost:7071/GraphQL"
#else
    let GraphQLEndPoint = "https://test.azurewebsites.net/GraphQL"
#endif

#endif
