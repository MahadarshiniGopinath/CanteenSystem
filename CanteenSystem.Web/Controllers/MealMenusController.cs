using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CanteenSystem.Web.Models;
using IdentityModel;

namespace CanteenSystem.Web.Controllers
{
    [ClaimRequirement(JwtClaimTypes.Role, "Admin")]
    public class MealMenusController : Controller
    {
        private readonly CanteenSystemDbContext _context;


        public MealMenusController(CanteenSystemDbContext context)
        {
            _context = context;
        }

        // GET: MealMenus
        public async Task<IActionResult> Index()
        {
            var canteenSystemDbContext = _context.MealMenus.Include(m => m.Discount).Include(m => m.MealType);
            return View(await canteenSystemDbContext.ToListAsync());
        }

        // GET: MealMenus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus
                .Include(m => m.Discount)
                .Include(m => m.MealType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenu == null)
            {
                return NotFound();
            }

            return View(mealMenu);
        }

        // GET: MealMenus/Create
        public IActionResult Create()
        {
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description");
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name");
            return View();
        }

        // POST: MealMenus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MealName,MealTypeId,Price,DiscountId")] MealMenu mealMenu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mealMenu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // GET: MealMenus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus.FindAsync(id);
            if (mealMenu == null)
            {
                return NotFound();
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // POST: MealMenus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MealName,MealTypeId,Price,DiscountId")] MealMenu mealMenu)
        {
            if (id != mealMenu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealMenu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealMenuExists(mealMenu.Id))
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
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "Id", "Description", mealMenu.DiscountId);
            ViewData["MealTypeId"] = new SelectList(_context.MealTypes, "Id", "Name", mealMenu.MealTypeId);
            return View(mealMenu);
        }

        // GET: MealMenus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mealMenu = await _context.MealMenus
                .Include(m => m.Discount)
                .Include(m => m.MealType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mealMenu == null)
            {
                return NotFound();
            }

            return View(mealMenu);
        }

        // POST: MealMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mealMenu = await _context.MealMenus.FindAsync(id);
            _context.MealMenus.Remove(mealMenu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MealMenuExists(int id)
        {
            return _context.MealMenus.Any(e => e.Id == id);
        }
    }
}
