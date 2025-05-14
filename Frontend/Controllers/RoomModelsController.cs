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
    public class RoomModelsController : Controller
    {
        private readonly IRoomApiService _roomService;

        public RoomModelsController(IRoomApiService roomService)
        {
            _roomService = roomService;
        }

        // GET: RoomModels
        public async Task<IActionResult> Index()
        {
            return View(await _roomService.GetRoomsAsync());
        }

        private async Task<IActionResult> GetRoomActionResult(int? id)
        {
            if (id == null) return NotFound();

            var model = await _roomService.GetRoomAsync(id.Value);

            return model == null ? NotFound() : View(model);
        }

        // GET: RoomModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetRoomActionResult(id);
        }

        // GET: RoomModels/Create
        public IActionResult Create()
        {
            //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            //ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID");
            return View();
        }

        // POST: RoomModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomID,Capacity,DepartmentID,EquipmentID")] RoomModel room)
        {
            if (!ModelState.IsValid)
            {
                //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
                //ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);

                return View(room);
            }

            bool response = await _roomService.AddRoomAsync(room);

            if(!response)
            {
                //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
                //ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);

                return View(room);
            }
            return RedirectToAction(nameof(Index));

        }

        // GET: RoomModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetRoomActionResult(id);
        }

        // POST: RoomModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomID,Capacity,DepartmentID,EquipmentID")] RoomModel room)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
                //ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);

                return View(room);
            }

            var response = await _roomService.UpdateRoomAsync(id, room);
            
            if (!response)
            {
                //ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
                //ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);

                return View(room);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: RoomModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetRoomActionResult(id);
        }

        // POST: RoomModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _roomService.DeleteRoomAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));
        }

    }
}
