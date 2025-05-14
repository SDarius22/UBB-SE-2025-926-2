using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Frontend.DbContext;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class ShiftModelsController : Controller
    {
        private readonly IShiftsApiService _shiftService;

        public ShiftModelsController(IShiftsApiService shiftService)
        {
            _shiftService = shiftService;
        }

        // GET: ShiftModels
        public async Task<IActionResult> Index()
        {
            return View(await _shiftService.GetShiftsAsync());
        }

        private async Task<IActionResult> GetShiftActionResult(int? id)
        {
            if (id == null) return NotFound();

            var model = await _shiftService.GetShiftAsync(id.Value);

            return model == null ? NotFound() : View(model);
        }

        // GET: ShiftModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetShiftActionResult(id);
        }

        // GET: ShiftModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShiftModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftID,Date,StartTime,EndTime")] ShiftModel shift)
        {
            if (!ModelState.IsValid)
            {
                return View(shift);
            }

            bool response = await _shiftService.AddShiftAsync(shift);

            return response ? RedirectToAction(nameof(Index)) : View(shift);

        }

        // GET: ShiftModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetShiftActionResult(id);
        }

        // POST: ShiftModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShiftID,Date,StartTime,EndTime")] ShiftModel shift)
        {
            if (id != shift.ShiftID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(shift);
            }

            var response = await _shiftService.UpdateShiftAsync(id, shift);

            return response ? RedirectToAction(nameof(Index)) : View(shift);
        }

        // GET: ShiftModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetShiftActionResult(id);
        }

        // POST: ShiftModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _shiftService.DeleteShiftAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));
        }
    }
}
