namespace Brimborium.OAuthDiagnostics.Model;

public sealed class LoggingState {
    public int Duration {
        get { return (int)Math.Ceiling(EnabledUntil.Subtract(System.DateTime.UtcNow).TotalHours); }
        set { EnabledUntil = System.DateTime.UtcNow.AddHours(Math.Clamp(value, 0, 24)); }
    }

    public DateTime EnabledUntil { get; set; }

    public bool IsEnabled { get; set; }

    public bool GetIsCurrentlyEnabled(DateTime utcNow) {
        if (!IsEnabled) { return false; }
        return (utcNow < EnabledUntil);
    }
}
