using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManager.Core.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }
}
