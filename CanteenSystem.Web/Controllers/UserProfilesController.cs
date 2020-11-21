﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CanteenSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using CanteenSystem.Web.ViewModel;
using Microsoft.AspNetCore.Identity;
using IdentityModel;

namespace CanteenSystem.Web.Controllers
{
    [ClaimRequirement(JwtClaimTypes.Role, "Admin")]
    
    public class UserProfilesController : Controller
    {
        private readonly CanteenSystemDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserProfilesController(CanteenSystemDbContext context,
             UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: UserProfiles
        public async Task<IActionResult> Index()
        {
            var canteenSystemDbContext = _context.UserProfiles.Include(u => u.ApplicationUser);
            var userModels = new List<UserModel>();
            var userProfiles = await canteenSystemDbContext.ToListAsync();
            foreach (var item in userProfiles)
            {
                 var userRole = await userManager.GetRolesAsync(item.ApplicationUser);
                userModels.Add(ConvertUserToUserProfile(item, userRole.FirstOrDefault()));
            }  
            return View(userModels);
        }

        // GET: UserProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
            {
                return NotFound();
            }
            var userRole = await userManager.GetRolesAsync(userProfile.ApplicationUser);
            return View(ConvertUserToUserProfile(userProfile, userRole?.First()));
        }

       
        // GET: UserProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.ApplicationUserId);
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EmailAddress,RollNumber,Department,IsVerified,ApplicationUserId")] UserProfile userProfile)
        {
            if (id != userProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userProfile);
                    await _context.SaveChangesAsync();

                   var applicationUser = await userManager.FindByIdAsync(userProfile.ApplicationUserId);
                    if (applicationUser != null)
                    {
                        applicationUser.UserName = userProfile.EmailAddress;
                        await userManager.UpdateAsync(applicationUser);
                    } 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userProfile.ApplicationUserId);
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userProfile = await _context.UserProfiles
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
          

            if (userProfile == null)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            _context.UserProfiles.Remove(userProfile);
           

            var applicationUser = await userManager.FindByIdAsync(userProfile.ApplicationUserId);
          
            if (applicationUser != null)
            { 
                await userManager.DeleteAsync(applicationUser);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }

        private UserModel ConvertUserToUserProfile(UserProfile user,string role) {


            return new UserModel { 
            ApplicationUserId = user.ApplicationUserId,
            Department = user.Department,
            EmailAddress = user.EmailAddress,
            IsVerified = user.IsVerified,
            Name = user.Name,
            RollNumber = user.RollNumber,
            Role = role,
            Id = user.Id
            };
        }
    }
}