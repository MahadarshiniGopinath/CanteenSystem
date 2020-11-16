using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CanteenSystem.Web.Models;
using CanteenSystem.Web.ViewModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CanteenSystem.Web.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly CanteenSystemDbContext _context;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        public LoginController(CanteenSystemDbContext context,

            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _context = context;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(new LoginModel());
        }
    }
}
