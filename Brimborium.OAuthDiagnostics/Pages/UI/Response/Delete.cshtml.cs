using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Response {
    public class DeleteModel : PageModel {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;
        private readonly DynamicEndpointDataSource _dynamicEndpointDataSource;

        public DeleteModel(
            Brimborium.OAuthDiagnostics.Model.DiaContext context,
            DynamicEndpointDataSource dynamicEndpointDataSource
            ) {
            this._context = context;
            this._dynamicEndpointDataSource = dynamicEndpointDataSource;
        }

        [BindProperty]
        public Brimborium.OAuthDiagnostics.Model.Response Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id) {
            if (id == null) {
                return this.NotFound();
            }

            var response = await this._context.DbSetResponse.FirstOrDefaultAsync(m => m.ResponseId == id);

            if (response is not null) {
                this.Data = response;

                return this.Page();
            }

            return this.NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id) {
            if (id == null) {
                return this.NotFound();
            }

            var response = await this._context.DbSetResponse.FindAsync(id);
            if (response != null) {
                this.Data = response;
                this._context.DbSetResponse.Remove(this.Data);
                await this._context.SaveChangesAsync();
                this._dynamicEndpointDataSource.Update(this._context.DbSetResponse.ToList());
            }

            return this.RedirectToPage("./Index");
        }
    }
}
