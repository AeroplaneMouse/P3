using AMS.Models;
using System;
using System.Collections.Generic;

namespace AMS.Logging.Interfaces
{
    public interface ILogger
    {
        bool AddEntry(Model entity, ulong userId);
        bool AddEntry(string inputEntryType, string inputDescription, ulong userId = 0, string changes = "[]", Exception e = null);
    }
}