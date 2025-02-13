namespace Brimborium.OAuthDiagnostics;

public class AppConfiguration     {
    /// <summary>
    /// if relative then LocalApplicationData /Brimborium/Brimborium.OAuthDiagnostics (or %HOME%) is used
    /// if empty then InMemory
    /// </summary>
    public string DatabaseFilename { get; set; } = string.Empty;
}