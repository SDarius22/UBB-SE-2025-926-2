using Backend.Configs;
using Backend.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentModel = Backend.Models.DocumentModel;

namespace Backend.DatabaseServices
{
    public class DocumentDatabaseService : IDocumentDatabaseService
    {
        private readonly AppDbContext _context;

        public DocumentDatabaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UploadDocumentToDataBase(DocumentModel document)
        {
            try
            {
                var entity = new DocumentModel(document.DocumentId, document.MedicalRecordId, document.Files);

                await _context.Documents.AddAsync(entity);
                await _context.SaveChangesAsync();

                document.DocumentId = entity.DocumentId;

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public async Task<List<DocumentModel>> GetDocumentsByMedicalRecordId(int medicalRecordId)
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.MedicalRecordId == medicalRecordId)
                    .Select(d => new DocumentModel(
                            d.DocumentId,
                            d.MedicalRecordId,
                            d.Files
                        ))
                    .ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception("Error loading documents: {exception.Message}");
            }
        }
    }
}
