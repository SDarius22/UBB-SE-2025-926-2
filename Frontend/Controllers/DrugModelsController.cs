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
    public class DrugModelsController : Controller
    {
        private readonly IDrugsApiService _drugsService;

        public DrugModelsController(IDrugsApiService drugsService)
        {
            _drugsService = drugsService;
        }

        private async Task<IActionResult> GetDrugActionResult(int? id)
        {
            if (id == null) return NotFound();

            var drug = await _drugsService.GetDrugAsync(id.Value);

            return drug == null ? NotFound() : View(drug);
        }

        // GET: DrugModels
        public async Task<IActionResult> Index()
        {
            return View(await _drugsService.GetDrugsAsync());
        }

        // GET: DrugModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetDrugActionResult(id);
        }

        // GET: DrugModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DrugModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DrugID,Name,Administration,Specification,Supply")] DrugModel drug)
        {
            if (!ModelState.IsValid)
            {
                return View(drug);
            }

            bool response = await _drugsService.AddDrugAsync(drug);

            return response ? RedirectToAction(nameof(Index)) : View(drug);
        }

        // GET: DrugModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetDrugActionResult(id);
        }

        // POST: DrugModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DrugID,Name,Administration,Specification,Supply")] DrugModel drug)
        {
            if (id != drug.DrugID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(drug);
            }

            var response = await _drugsService.UpdateDrugAsync(id, drug);

            return response ? RedirectToAction(nameof(Index)) : View(drug);
        }

        // GET: DrugModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetDrugActionResult(id);
        }

        // POST: DrugModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _drugsService.DeleteDrugAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));
        }

    }
}
