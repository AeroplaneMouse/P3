using AMS.Models;
using System;
using System.Collections.Generic;

namespace AMS.Logging.Interfaces
{
    public interface Ilogger
    {
        bool AddEntry(Model entity, ulong userId, ulong entityId = 0);
        bool AddEntry(Exception e);
        bool AddEntry(string inputEntryType, string inputDescription, ulong userId = 0, string changes = "[]");
    }
}