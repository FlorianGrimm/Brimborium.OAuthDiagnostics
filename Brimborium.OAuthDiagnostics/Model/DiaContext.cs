using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Brimborium.OAuthDiagnostics.Model;

public class DiaContext : DbContext {
    public DbSet<Response> DbSetResponse { get; set; }
    public DbSet<Request> DbSetRequest { get; set; }

    public string DbPath { get; set; } = string.Empty;

    public DiaContext() {
    }

    public DiaContext(IOptions<AppConfiguration> options) {
        this.DbPath = options.Value.DatabaseFilename;
    }

    public static string GetFullFileName(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return string.Empty;
        }
        if (path is { Length: > 0 } && path.Contains('%')) {
            path = Environment.ExpandEnvironmentVariables(path);
        }
        if (!Path.IsPathFullyQualified(path)) {
            string basePath;
            if (System.Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") is { Length: > 0 }
              && System.Environment.GetEnvironmentVariable("HOME") is { Length: > 0 } home) {
                basePath = Environment.ExpandEnvironmentVariables("""%HOME%\data""");
            } else if (OperatingSystem.IsWindows()) {
                basePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Brimborium",
                    "Brimborium.OAuthDiagnostics");
            } else {
                basePath = Environment.ExpandEnvironmentVariables("%HOME%");
            }
            if (path == "default") { path = "Brimborium.OAuthDiagnostics.db"; }
            path = Path.Join(basePath, path);
        }
        return path;
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options) {
        if (this.DbPath is { Length: > 0 }) {
            options.UseSqlite($"Data Source={this.DbPath}");
        } else {
            options.UseInMemoryDatabase("Brimborium.OAuthDiagnostics");
        }
    }
}

public class Response {
    [Key]
    public int ResponseId { get; set; }
    public string Path { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    public string ContentType { get; set; } = string.Empty;
    public string ContentBody { get; set; } = string.Empty;
}

public class Request {
    [Key]
    public int RequestId { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Headers { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class Configuration {
    [Key]
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class LoggedRequest {
    public int LoggedRequestId { get; set; }
    public DateTime At { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Headers { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}