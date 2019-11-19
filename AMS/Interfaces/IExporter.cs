using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Interfaces
{
    public interface IExporter
    {
        void Print(IEnumerable<object> items);
    }
}
