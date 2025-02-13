using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Brimborium.OAuthDiagnostics.Model;

namespace Brimborium.OAuthDiagnostics.Pages.UI.Response
{
    public class IndexModel : PageModel
    {
        private readonly Brimborium.OAuthDiagnostics.Model.DiaContext _context;

        public IndexModel(Brimborium.OAuthDiagnostics.Model.DiaContext context)
        {
            this._context = context;
        }

        public IList<Brimborium.OAuthDiagnostics.Model.Response> Data { get;set; } = default!;

        public async Task OnGetAsync()
        {
            this.Data = await this._context.DbSetResponse.ToListAsync();
        }
    }
}
