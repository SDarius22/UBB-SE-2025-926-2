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
    public class EquipmentModelsController : Controller
    {
        private readonly IEquipmentApiService _equipmentService;

        public EquipmentModelsController(IEquipmentApiService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        private async Task<IActionResult> GetEquipmentActionResult(int? id)
        {
            if (id == null) return NotFound();

            var model = await _equipmentService.GetEquipmentAsync(id.Value);

            return model == null ? NotFound() : View(model);
        }

        // GET: EquipmentModels
        public async Task<IActionResult> Index()
        {
            return View(await _equipmentService.GetEquipmentsAsync());
        }

        // GET: EquipmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetEquipmentActionResult(id);
        }

        // GET: EquipmentModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipmentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentID,Name,Type,Specification,Stock")] EquipmentModel equipment)
        {
            if (!ModelState.IsValid)
            {
                return View(equipment);
            }

            bool response = await _equipmentService.AddEquipmentAsync(equipment);

            return response ? RedirectToAction(nameof(Index)) : View(equipment);
        }

        // GET: EquipmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetEquipmentActionResult(id);
        }

        // POST: EquipmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipmentID,Name,Type,Specification,Stock")] EquipmentModel equipment)
        {
            if (id != equipment.EquipmentID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(equipment);
            }

            var response = await _equipmentService.UpdateEquipmentAsync(id, equipment);

            return response ? RedirectToAction(nameof(Index)) : View(equipment);
        }

        // GET: EquipmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetEquipmentActionResult(id);
        }

        // POST: EquipmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _equipmentService.DeleteEquipmentAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));
        }

    }
}
