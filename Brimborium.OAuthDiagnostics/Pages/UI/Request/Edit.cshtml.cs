using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Brimborium.OAuthDiagnostics.Model;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Request
{
    public class EditModel : PageModel
    {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;

        public EditModel(Brimborium.OAuthDiagnostics.Model.DiaContext context)
        {
            this._context = context;
        }

        [BindProperty]
        public Brimborium.OAuthDiagnostics.Model.Request Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var request =  await this._context.DbSetRequest.FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return this.NotFound();
            }
            this.Data = request;
            return this.Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            this._context.Attach(this.Data).State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.RequestExists(this.Data.RequestId))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.RedirectToPage("./Index");
        }

        private bool RequestExists(int id)
        {
            return this._context.DbSetRequest.Any(e => e.RequestId == id);
        }
    }
}
