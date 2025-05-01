using System;

namespace Backend.Exceptions
{
    public class CancellationNotAllowedException : Exception
    {
        public CancellationNotAllowedException(string message) : base(message) { }
    }
}
