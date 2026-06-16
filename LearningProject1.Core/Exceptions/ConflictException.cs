using System;
using System.Collections.Generic;
using System.Text;

namespace LearningProject1.Core.Exceptions;

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
