﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Exceptions
{
    public class InvalidAppointmentException : Exception
    {
        public InvalidAppointmentException(string message) : base(message)
        {
        }
    }
}
