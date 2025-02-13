using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Brimborium.OAuthDiagnostics.Model;
using Brimborium.OAuthDiagnostics.Service;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Response {
    public class EditModel : PageModel {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;
        private readonly DynamicEndpointDataSource _dynamicEndpointDataSource;

        public EditModel(
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
            if (response == null) {
                return this.NotFound();
            }
            this.Data = response;
            return this.Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync() {
            if (!this.ModelState.IsValid) {
                return this.Page();
            }

            this._context.Attach(this.Data).State = EntityState.Modified;

            try {
                await this._context.SaveChangesAsync();
                this._dynamicEndpointDataSource.Update(this._context.DbSetResponse.ToList());
            } catch (DbUpdateConcurrencyException) {
                if (!this.ResponseExists(this.Data.ResponseId)) {
                    return this.NotFound();
                } else {
                    throw;
                }
            }

            return this.RedirectToPage("./Index");
        }

        private bool ResponseExists(int id) {
            return this._context.DbSetResponse.Any(e => e.ResponseId == id);
        }
    }
}
