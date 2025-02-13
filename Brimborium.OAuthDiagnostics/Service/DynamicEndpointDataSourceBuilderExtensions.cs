using Brimborium.OAuthDiagnostics.Service;

namespace Microsoft.AspNetCore.Builder;

public static class DynamicEndpointDataSourceBuilderExtensions {
    public static void UseDynamicEndpoints(this IEndpointRouteBuilder endpoints) {
        var dynamicEndpointDataSource = endpoints.ServiceProvider.GetRequiredService<DynamicEndpointDataSource>();
        endpoints.DataSources.Add(dynamicEndpointDataSource);
    }
}