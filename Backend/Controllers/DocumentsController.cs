using Backend.DatabaseServices;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentDatabaseService _documentService;

        public DocumentsController(IDocumentDatabaseService documentService)
        {
            _documentService = documentService;
        }

        // POST: api/documents
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromBody] DocumentModel document)
        {
            var success = await _documentService.UploadDocumentToDataBase(document);
            if (!success)
                return StatusCode(500, "Failed to upload document");

            return CreatedAtAction(nameof(GetDocumentsByMedicalRecordId), new { medicalRecordId = document.MedicalRecordId }, document);
        }

        // GET: api/documents/medicalRecord/{medicalRecordId}
        [HttpGet("medicalRecord/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<DocumentModel>>> GetDocumentsByMedicalRecordId(int medicalRecordId)
        {
            var documents = await _documentService.GetDocumentsByMedicalRecordId(medicalRecordId);
            return Ok(documents);
        }
    }
}
