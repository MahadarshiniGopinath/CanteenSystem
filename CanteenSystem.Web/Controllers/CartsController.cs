﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CanteenSystem.Web.Models;
using Newtonsoft.Json;

namespace CanteenSystem.Web.Controllers
{
    public class CartsController : Controller
    {
        private readonly CanteenSystemDbContext _context;

        public CartsController(CanteenSystemDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index(int userProfileId)
        {
            var canteenSystemDbContext = _context.Carts.Include(c => c.MealMenu).Include(c => c.UserProfile);
            var cartList = await canteenSystemDbContext.ToListAsync();
            cartList = cartList.Where(x => x.UserProfileId == userProfileId)?.ToList();
            return View(cartList);
        }

        [Route("Carts/AddToCart/{menuId}/{availabilityDateId}/{userProfileId}")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int menuId, int availabilityDateId, int userProfileId)
        {
            var canteenSystemDbContext = _context.MealMenus.Where(x => x.Id == menuId &&
            x.MealMenuAvailabilities.Any(y => y.Id == availabilityDateId)).Include(x => x.MealMenuAvailabilities);
            var mealMenu = await canteenSystemDbContext.FirstOrDefaultAsync();
            if (mealMenu != null)
            {
                var cart = new Cart
                {

                    MealMenuId = mealMenu.Id,
                    MealAvailableDate = mealMenu.MealMenuAvailabilities.Where(x => x.Id == availabilityDateId).FirstOrDefault().AvailabilityDate,
                    Price = mealMenu.Price,
                    Quantity = 1,
                    UserProfileId = userProfileId
                };
                _context.Add(cart);
                _context.SaveChanges();
            }

            return Json(new { Status = true });
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.MealMenu)
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName");
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealMenuId,Quantity,Price,CreatedDate,UpdatedDate,MealAvailableDate,UserProfileId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MealMenuId,Quantity,Price,CreatedDate,UpdatedDate,MealAvailableDate,UserProfileId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["MealMenuId"] = new SelectList(_context.MealMenus, "Id", "MealName", cart.MealMenuId);
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "EmailAddress", cart.UserProfileId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.MealMenu)
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}