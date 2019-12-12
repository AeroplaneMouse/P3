using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace AMS.Interfaces
{
    public interface ITagable
    {
        ulong TagId { get; }
        Type TagType { get; }
        string TagLabel { get; }
        string FullTagLabel { get; set; }
        ulong ParentId { get; }
        int NumberOfChildren { get; }
        List<ITagable> Children { get; set; }
        string TagColor { get; set; }
        SolidColorBrush TagFontColor { get; }
    }
}