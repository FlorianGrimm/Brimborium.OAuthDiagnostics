using Brimborium.OAuthDiagnostics.Service;

namespace Microsoft.Extensions.DependencyInjection;

public static class DynamicEndpointDataSourceServiceCollectionExtensions {
    public static void AddDynamicEndpoints(this IServiceCollection services, DynamicEndpointDataSource? instance = default) {
        if (instance is { }) {
            services.AddSingleton<DynamicEndpointDataSource>(instance);
        } else {
            services.AddSingleton<DynamicEndpointDataSource>();
        }
        services.AddSingleton<LoggingRequestService>();
    }
}
