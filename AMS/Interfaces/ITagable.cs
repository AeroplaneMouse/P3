﻿using System;
using System.Collections.Generic;

namespace AMS.Interfaces
{
    public interface ITagable
    {
        ulong TagId { get; }
        Type TagType { get; }
        string TagLabel { get; }
        List<ITagable> Children { get; set; }
    }
}