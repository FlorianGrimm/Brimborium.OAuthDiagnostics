using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Response {
    public class CreateModel : PageModel {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;
        private readonly DynamicEndpointDataSource _dynamicEndpointDataSource;

        public CreateModel(
            Brimborium.OAuthDiagnostics.Model.DiaContext context,
            DynamicEndpointDataSource dynamicEndpointDataSource
            ) {
            this._context = context;
            this._dynamicEndpointDataSource = dynamicEndpointDataSource;
        }

        public IActionResult OnGet() {
            return this.Page();
        }

        [BindProperty]
        public Brimborium.OAuthDiagnostics.Model.Response Data { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) {
                return this.Page();
            }

            this._context.DbSetResponse.Add(this.Data);
            await this._context.SaveChangesAsync();
            this._dynamicEndpointDataSource.Update(this._context.DbSetResponse.ToList());
            return this.RedirectToPage("./Index");
        }
    }
}
