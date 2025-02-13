using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Brimborium.OAuthDiagnostics.Model;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Request
{
    public class DetailsModel : PageModel
    {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;

        public DetailsModel(Brimborium.OAuthDiagnostics.Model.DiaContext context)
        {
            this._context = context;
        }

        public Brimborium.OAuthDiagnostics.Model.Request Data { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var request = await this._context.DbSetRequest.FirstOrDefaultAsync(m => m.RequestId == id);

            if (request is not null)
            {
                this.Data = request;

                return this.Page();
            }

            return this.NotFound();
        }
    }
}
