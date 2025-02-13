using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Brimborium.OAuthDiagnostics.Pages.UI {
    public class DetailsModel : PageModel {
        public DetailsModel(
            LoggingState loggingState,
            LoggingRequestService loggingRequestService
            ) {
            this.LoggingState = loggingState;
            this._LoggingRequestService = loggingRequestService;
            this.Data = new();
        }

        public LoggingState LoggingState { get; private set; }
        public LoggedRequest Data { get; set; }

        private LoggingRequestService _LoggingRequestService;

        public void OnGet(int? id) {
            if (!id.HasValue) {
                this.Data = new() { Headers = "not found" };
            } else {
                this.Data = _LoggingRequestService.GetLoggedRequest(id.Value) ?? new LoggedRequest();
            }
        }
    }
}
