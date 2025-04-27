using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    public class ShiftNotFoundException : Exception
    {
        public ShiftNotFoundException(string message) : base(message) { }
    }
}
