using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DemoException : Exception
{
    public DemoException(string message)
        : base(message)
    {
    }
}
