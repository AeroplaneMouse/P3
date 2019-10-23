﻿using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Asset_Management_System.Database.Repositories
{
    interface ILogRepository<T> : ISearchableRepository<T>
    {    
        bool Insert(T entity);
        
        IEnumerable<T> GetLogEntries(ulong logable_id, Type logable_type);
        IEnumerable<T> GetLogEntries(ulong logable_id, Type logable_type, string username);
    }
}