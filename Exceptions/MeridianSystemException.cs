using System;
using System.Collections.Generic;
using System.Text;

namespace MeridianGroundOperationsManagementSystem.Exceptions
{
    public class MeridianSystemException : Exception
    {
        public MeridianSystemException(string? message) : base(message)
        {
        }
    }
}
