//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Frontend.DbContext;
//using Frontend.Models;

//// Asta nu trebuie nicaieri

//namespace Frontend.Controllers
//{
//    public class DocumentModelsController : Controller
//    {
//        private readonly AppDbContext _context;

//        public DocumentModelsController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: DocumentModels
//        public async Task<IActionResult> Index()
//        {
//            var appDbContext = _context.Documents.Include(d => d.MedicalRecord);
//            return View(await appDbContext.ToListAsync());
//        }

//        // GET: DocumentModels/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var documentModel = await _context.Documents
//                .Include(d => d.MedicalRecord)
//                .FirstOrDefaultAsync(m => m.DocumentId == id);
//            if (documentModel == null)
//            {
//                return NotFound();
//            }

//            return View(documentModel);
//        }

//        // GET: DocumentModels/Create
//        public IActionResult Create()
//        {
//            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId");
//            return View();
//        }

//        // POST: DocumentModels/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("DocumentId,MedicalRecordId,Files")] DocumentModel documentModel)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(documentModel);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", documentModel.MedicalRecordId);
//            return View(documentModel);
//        }

//        // GET: DocumentModels/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var documentModel = await _context.Documents.FindAsync(id);
//            if (documentModel == null)
//            {
//                return NotFound();
//            }
//            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", documentModel.MedicalRecordId);
//            return View(documentModel);
//        }

//        // POST: DocumentModels/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("DocumentId,MedicalRecordId,Files")] DocumentModel documentModel)
//        {
//            if (id != documentModel.DocumentId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(documentModel);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!DocumentModelExists(documentModel.DocumentId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", documentModel.MedicalRecordId);
//            return View(documentModel);
//        }

//        // GET: DocumentModels/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var documentModel = await _context.Documents
//                .Include(d => d.MedicalRecord)
//                .FirstOrDefaultAsync(m => m.DocumentId == id);
//            if (documentModel == null)
//            {
//                return NotFound();
//            }

//            return View(documentModel);
//        }

//        // POST: DocumentModels/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var documentModel = await _context.Documents.FindAsync(id);
//            if (documentModel != null)
//            {
//                _context.Documents.Remove(documentModel);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool DocumentModelExists(int id)
//        {
//            return _context.Documents.Any(e => e.DocumentId == id);
//        }
//    }
//}
