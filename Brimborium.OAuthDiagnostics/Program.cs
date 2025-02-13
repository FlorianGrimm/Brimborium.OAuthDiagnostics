using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

using Microsoft.EntityFrameworkCore;

namespace Brimborium.OAuthDiagnostics;
public class Program {
    private static bool _Restart = false;
    public static void Restart(IHostApplicationLifetime hostApplicationLifetime) {
        _ = Task.Factory.StartNew(() => { restart(); });
        async void restart() {
            await Task.Delay(100);
            _Restart = true;
            hostApplicationLifetime.StopApplication();
        }
    }
    public static async Task Main(string[] args) {
        while (true) {
            await Run(args);
            if (_Restart) {
                _Restart = false;
                continue;
            } else {
                break;
            }
        }
    }
    public static async Task Run(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions<AppConfiguration>().BindConfiguration("Application");
        builder.Services.AddSingleton<LoggingState>(new LoggingState() { Duration=8, IsEnabled=false });
        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
        builder.Logging.AddLocalFile(
          configure: (options) => {
              if (System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") is { Length: > 0 }
                  && System.Environment.GetEnvironmentVariable("HOME") is { Length: > 0 } home) {
                  options.BaseDirectory = home;
                  if (string.IsNullOrEmpty(options.LogDirectory)) {
                      options.LogDirectory = "LogFiles\\Application";
                  }
                  System.Console.Out.WriteLine($"Azure: {options.BaseDirectory}/{options.LogDirectory}");
              } else {
                  if (string.IsNullOrEmpty(options.BaseDirectory)) {
                      options.BaseDirectory = builder.Environment.ContentRootPath;
                  }
                  System.Console.Out.WriteLine($"OnPremises: {options.BaseDirectory}/{options.LogDirectory}");
              }

          },
          configuration: builder.Configuration.GetSection("Logging:LocalFile"));

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<DiaContext>();
        builder.Services.AddDynamicEndpoints();

        var app = builder.Build();

        {
            using var scope = app.Services.CreateScope();
            var diaContext = scope.ServiceProvider.GetRequiredService<DiaContext>();
            await diaContext.Database.EnsureCreatedAsync();

            var dynamicEndpointDataSource = scope.ServiceProvider.GetRequiredService<DynamicEndpointDataSource>();
            var listResponse = diaContext.DbSetResponse.ToList();
            if (listResponse.Count == 0) {
                diaContext.DbSetResponse.Add(
                    new Response() {
                        ResponseId = 0,
                        Path = "/{**rest}",
                        StatusCode = 200,
                        ContentType = "application/json",
                        ContentBody = "[]"
                    });
                await diaContext.SaveChangesAsync();

                listResponse = diaContext.DbSetResponse.ToList();
            }
            dynamicEndpointDataSource.Update(listResponse);
        }
        /*
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        */

        app.UseHttpsRedirection();


        app.UseRouting();

        app.UseAuthorization();

        app.UseDynamicEndpoints();
        app.UseEndpoints(_ => { });
        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.Run();
    }
}
