using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CanteenSystem.UI.Models;

namespace CanteenSystem.UI
{
    public class IndexModel : PageModel
    {
        private readonly CanteenSystem.UI.Models.CanteenSystemDbContext _context;

        public IndexModel(CanteenSystem.UI.Models.CanteenSystemDbContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; }

        public async Task OnGetAsync()
        {
            User = await _context.Users.ToListAsync();
        }
    }
}
