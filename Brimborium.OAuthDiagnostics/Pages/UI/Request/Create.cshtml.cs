using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Brimborium.OAuthDiagnostics.Model;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Request
{
    public class CreateModel : PageModel
    {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;

        public CreateModel(Brimborium.OAuthDiagnostics.Model.DiaContext context)
        {
            this._context = context;
        }

        public IActionResult OnGet()
        {
            return this.Page();
        }

        [BindProperty]
        public Brimborium.OAuthDiagnostics.Model.Request Data { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            this._context.DbSetRequest.Add(this.Data);
            await this._context.SaveChangesAsync();

            return this.RedirectToPage("./Index");
        }
    }
}
