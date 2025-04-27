using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public int MedicalRecordId { get; set; }
        public string Files { get; set; }


        public DocumentModel(int documentId, int medicalRecordId, string files)
        {
            DocumentId = documentId;
            MedicalRecordId = medicalRecordId;
            Files = files;
        }
    }
}
