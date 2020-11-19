using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CanteenSystem.Web.Models;
using CanteenSystem.Web.ViewModel;
using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CanteenSystem.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly CanteenSystemDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        public LoginController(CanteenSystemDbContext context,

            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [Route("login")]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userProfile = _context.UserProfiles.Where(x => x.EmailAddress == model.Username)
                    .FirstOrDefault();
                if (userProfile != null && userProfile.IsVerified)
                {
                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userProfile.Name));
                    var userRoles = await userManager.GetRolesAsync(user);

                    var role = userRoles.FirstOrDefault();
                    if (role != null)
                    {
                        identity.AddClaim(new Claim(JwtClaimTypes.Role, role));
                    }
                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                        new ClaimsPrincipal(identity));
                    return RedirectToLocal(returnUrl);

                }
                else
                {
                    ModelState.AddModelError("", "Invalid UserName or Password");
                    return View();
                }
            }
            ModelState.AddModelError("", "Invalid UserName or Password");
            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }

        [Route("register")]
        public IActionResult Register()
        {
            return View(new RegisterModel());
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            var userExists = await userManager.FindByNameAsync(model.Email);

            var existingRoleNumber = _context.UserProfiles.Where(x => x.RollNumber == model.Rollnumber).FirstOrDefault();
            if (userExists != null || existingRoleNumber != null)
            {
                ModelState.AddModelError("error", "User already exists!");

                return View("Register", model);
            }
            var applicationUser = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("error", "User creation failed! Please check user details and try again.");

                return View("Register", model);
            }

            var newlyCreatedUser = await userManager.FindByNameAsync(model.Email);
            if (newlyCreatedUser != null)
            {

                var user = new UserProfile()
                {
                    Name = model.Firstname + " " + model.Lastname,
                    EmailAddress = model.Email,
                    Department = model.Department,
                    RollNumber = model.Rollnumber,
                    IsVerified = !model.IsParent,
                    ApplicationUserId = newlyCreatedUser.Id
                };
                _context.Add(user);
                _context.SaveChanges();
            }
            if (!model.IsParent)
            {
                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Student));

                if (await roleManager.RoleExistsAsync(UserRoles.Student))
                {
                    await userManager.AddToRoleAsync(newlyCreatedUser, UserRoles.Student);
                }
            }
            else
            {
                if (!await roleManager.RoleExistsAsync(UserRoles.Parents))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Parents));

                if (await roleManager.RoleExistsAsync(UserRoles.Parents))
                {
                    await userManager.AddToRoleAsync(newlyCreatedUser, UserRoles.Parents);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [Route("register/admin")]
        public IActionResult RegisterAdmin()
        {
            return View("Register", new RegisterModel(true));
        }


        [HttpPost]
        [Route("register/admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Email);

            var existingRoleNumber = _context.UserProfiles.Where(x => x.RollNumber == model.Rollnumber).FirstOrDefault();
            if (userExists != null || existingRoleNumber != null)
            {
                ModelState.AddModelError("error", "User already exists!");
                model.IsAdmin = true;
                return View("Register", model);
            }
            var applicationUser = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await userManager.CreateAsync(applicationUser, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("error", "User creation failed! Please check user details and try again.");
                model.IsAdmin = true;
                return View("Register", model);
            }

            var newlyCreatedUser = await userManager.FindByNameAsync(model.Email);
            if (newlyCreatedUser != null)
            {

                var user = new UserProfile()
                {
                    Name = model.Firstname + " " + model.Lastname,
                    EmailAddress = model.Email,
                    Department = model.Department,
                    RollNumber = model.Rollnumber,
                    IsVerified = !model.IsParent,
                    ApplicationUserId = newlyCreatedUser.Id
                };
                _context.Add(user);
                _context.SaveChanges();
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(newlyCreatedUser, UserRoles.Admin);
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    public class CustomClaimsFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {

        private readonly UserManager<ApplicationUser> _userManager;
        public CustomClaimsFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { _userManager = userManager; }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            var userRoles = await _userManager.GetRolesAsync(user);

            var role = userRoles.FirstOrDefault();
            if (role != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
            claims.Add(new Claim("UserName", user.UserName));
            identity.AddClaims(claims);
            return principal;
        }


    }



}

