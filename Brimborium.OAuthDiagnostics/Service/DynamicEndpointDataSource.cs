using Brimborium.OAuthDiagnostics.Model;

using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Primitives;

using System.Collections.Immutable;

namespace Brimborium.OAuthDiagnostics.Service;

public class DynamicEndpointDataSource : EndpointDataSource {
    private CancellationTokenSource _CancellationTokenSource;
    private CancellationChangeToken _ChangeToken;
    private ImmutableArray<Endpoint> _Endpoints;
    private readonly ILogger<DynamicEndpointDataSource> _Logger;

    public DynamicEndpointDataSource(ILogger<DynamicEndpointDataSource> logger) {
        this._CancellationTokenSource = new CancellationTokenSource();
        this._ChangeToken = new CancellationChangeToken(this._CancellationTokenSource.Token);
        this._Logger = logger;
        this._Endpoints = ImmutableArray<Endpoint>.Empty;
    }

    public override IReadOnlyList<Endpoint> Endpoints => this._Endpoints;

    public override IChangeToken GetChangeToken() => this._ChangeToken;

    public void Update(List<Brimborium.OAuthDiagnostics.Model.Response> listResponse) {
        {
            var builderEndpoint = ImmutableArray.CreateBuilder<Endpoint>();
            foreach (var response in listResponse) {
                RoutePattern routePattern;
                try {
                    routePattern = RoutePatternFactory.Parse(response.Path);
                } catch (System.Exception error) {
                    this._Logger.LogInformation(error, "Invalid Path: {RequestPath}", response.Path);
                    continue;
                }
                var endpoint = new RouteEndpointBuilder(
                        requestDelegate: CreateEnpointReturningContent(response),
                        routePattern: routePattern,
                        order: 100
                    ) {
                    DisplayName = response.Path
                }
                    .Build();
                builderEndpoint.Add(endpoint);
            }
            this._Endpoints = builderEndpoint.ToImmutable();
        }
        {
            var old = this._CancellationTokenSource;
            this._CancellationTokenSource = new CancellationTokenSource();
            this._ChangeToken = new CancellationChangeToken(this._CancellationTokenSource.Token);
            System.Threading.Interlocked.MemoryBarrier();
            old.Cancel();
        }
    }

    private RequestDelegate CreateEnpointReturningContent(Response response) {
        RequestDelegate fn = async (HttpContext httpContext) => {
            var httpResponse = httpContext.Response;
            var loggingRequestService = httpContext.RequestServices.GetRequiredService<LoggingRequestService>();
            await loggingRequestService.HandleRequestAsync(httpContext.Request);
            httpResponse.StatusCode = response.StatusCode;
            httpResponse.Headers.ContentType = response.ContentType;
            await httpResponse.WriteAsync(response.ContentBody);
        };
        return fn;
    }
}
