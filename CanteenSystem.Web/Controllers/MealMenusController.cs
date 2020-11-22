using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CanteenSystem.Web.Models;
using IdentityModel;
using CanteenSystem.Web.ViewModel;

namespace CanteenSystem.Web.Controllers
{ 
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

        // GET: MealMenus
        public async Task<IActionResult> StudentMealList(DateTime? selectedDate = null, int? mealType = null, int? discount = null)
        {
            var canteenSystemDbContext = _context.MealMenus.Include(m => m.Discount).Include(m => m.MealType)
                .Include(m => m.MealMenuAvailabilities);
            var listOfValues = await canteenSystemDbContext.ToListAsync();
           
            listOfValues = !selectedDate.HasValue?
                listOfValues.Where(x => x.MealMenuAvailabilities.Any(y => y.AvailabilityDate.Date >= DateTime.Now.Date)).ToList():
            listOfValues.Where(x => x.MealMenuAvailabilities.Any(y => y.AvailabilityDate.Date == selectedDate.Value.Date)).ToList();
            var availableMealTypes = _context.MealTypes.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            if (discount != null)
            {
                listOfValues = listOfValues.Where(y => y.DiscountId == discount).ToList();
            }
            if (mealType != null)
            {
                listOfValues = listOfValues.Where(y => y.MealTypeId == mealType).ToList();
            }
            var mealMenuCollection = new MealMenuCollectionModel
            {
                AvailableMealTypes = new SelectList(availableMealTypes,"Text","Value"),
                MealMenuModels = listOfValues.SelectMany(x => x.MealMenuAvailabilities).Select(x => {
                    decimal price = 0;
                    decimal? wasPrice = null;

                    if (x.MealMenu.DiscountId != null && x.AvailabilityDate >= x.MealMenu.Discount.ValidFromDate
                     && (x.MealMenu.Discount.ValidToDate == null || x.AvailabilityDate <= x.MealMenu.Discount.ValidToDate))
                    {
                        price = (decimal)((x.MealMenu.Price * x.MealMenu.Discount.DiscountPercentage) / 100);
                        wasPrice = (decimal)x.MealMenu.Price;
                    }
                    else {
                        price = (decimal)x.MealMenu.Price;
                    }
                    
                    return new MealMenuModel
                    {
                       Id = x.MealMenuId,
                       Name =x.MealMenu.MealName,
                       MealType = x.MealMenu.MealType.Name,
                       AvailableDate =x.AvailabilityDate,
                       Price = price,
                       WasPrice = wasPrice
                    };
                }).ToList()
            };
       
             
            return View(mealMenuCollection);
        }
    }
}
