
using Brimborium.OAuthDiagnostics.Model;

using Microsoft.AspNetCore.Http.Extensions;

using System.Text;

namespace Brimborium.OAuthDiagnostics.Service;

public class LoggingRequestService {
    private readonly LoggingState _LoggingState;
    private readonly List<LoggedRequest> _ListLoggedRequest = new();

    public LoggingRequestService(LoggingState loggingState) {
        this._LoggingState = loggingState;
    }

    public List<LoggedRequest> GetListLoggedRequest() {
        lock (this._ListLoggedRequest) {
            var result = new List<LoggedRequest>(this._ListLoggedRequest.Count);
            result.AddRange(this._ListLoggedRequest);
            return result;
        }
    }

    public async Task HandleRequestAsync(HttpRequest request) {
        var utcNow = System.DateTime.UtcNow;
        if (!this._LoggingState.GetIsCurrentlyEnabled(utcNow)) {
            return;
        }

        var header = GetHeaderContent(request);
        string content = await GetBodyContent(request);
        var loggedRequest = new LoggedRequest() {
            At = utcNow,
            Method = request.Method,
            Path = request.GetEncodedPathAndQuery(),
            Headers = header,
            Content = content
        };

        lock (this._ListLoggedRequest) {
            var count = this._ListLoggedRequest.Count;
            if (count == 0) {
                loggedRequest.LoggedRequestId = 1;
            } else {
                loggedRequest.LoggedRequestId = this._ListLoggedRequest[count - 1].LoggedRequestId + 1;
            }
            this._ListLoggedRequest.Add(loggedRequest);
        }
    }

    private static string GetHeaderContent(HttpRequest request) {
        var sbHeader = new StringBuilder();
        foreach (var header in request.Headers) {
            foreach (var value in header.Value) {
                sbHeader.AppendLine($"{header.Key}: {value}");
            }
        }

        return sbHeader.ToString();
    }

    private static async Task<string> GetBodyContent(HttpRequest request) {
        if (request.ContentLength is > 0) {
            try {
                using var reader = new StreamReader(request.Body);
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            } catch { }
        }
        return string.Empty;
    }

    public void DeleteLoggedRequest(int id) {
        lock (this._ListLoggedRequest) {
            var index = this._ListLoggedRequest.FindIndex((item) => item.LoggedRequestId == id);
            if (0 <= index) {
                this._ListLoggedRequest.RemoveAt(index);
            }
        }
    }

    public LoggedRequest? GetLoggedRequest(int id) {
        lock (this._ListLoggedRequest) {
            return this._ListLoggedRequest.Find((item) => item.LoggedRequestId == id);
        }
    }
}
