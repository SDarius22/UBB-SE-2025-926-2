using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    public class MedicalRecordNotFoundException : Exception
    {
        public MedicalRecordNotFoundException() : base() { }

        public MedicalRecordNotFoundException(string message) : base(message) { }
    }
}
