using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Brimborium.OAuthDiagnostics.Pages.UI
{
    public class IndexModel : PageModel
    {
        private readonly LoggingRequestService _LoggingRequestService;

        public IndexModel(
            LoggingState loggingState,
            LoggingRequestService loggingRequestService) {
            this.LoggingState = loggingState;
            this._LoggingRequestService = loggingRequestService;
        }

        public List<Brimborium.OAuthDiagnostics.Model.LoggedRequest> Data { get; set; } = new ();

        [BindProperty]
        public LoggingState LoggingState { get; }

        public void OnGet()
        {
            this.Data = this._LoggingRequestService.GetListLoggedRequest();   
        }

        public IActionResult OnPostDetails(int id) {
            return this.RedirectToPage();
        }

        public IActionResult OnPostDelete(int id) {
            this._LoggingRequestService.DeleteLoggedRequest(id);
            return this.RedirectToPage();
        }

        public IActionResult OnPostEnable() {
            this.LoggingState.IsEnabled = true;
            int.TryParse(this.ModelState["LoggingState.Duration"]?.AttemptedValue, out var duration);
            if (duration == 0) { duration = 1; }
            if (this.LoggingState.Duration != duration){ this.LoggingState.Duration = duration; }
            return this.RedirectToPage();
        }

        public IActionResult OnPostDisable() {
            this.LoggingState.IsEnabled = false;
            return this.RedirectToPage();
        }
    }
}
