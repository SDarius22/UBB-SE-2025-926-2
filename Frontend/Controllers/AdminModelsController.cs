using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Frontend.DbContext;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class AdminModelsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AdminModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Admins.Include(a => a.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AdminModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // GET: AdminModels/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role");
            return View();
        }

        // POST: AdminModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,UserId")] AdminModel adminModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", adminModel.UserId);
            return View(adminModel);
        }

        // GET: AdminModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admins.FindAsync(id);
            if (adminModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", adminModel.UserId);
            return View(adminModel);
        }

        // POST: AdminModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,UserId")] AdminModel adminModel)
        {
            if (id != adminModel.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminModelExists(adminModel.AdminId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", adminModel.UserId);
            return View(adminModel);
        }

        // GET: AdminModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // POST: AdminModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminModel = await _context.Admins.FindAsync(id);
            if (adminModel != null)
            {
                _context.Admins.Remove(adminModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminModelExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}
