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
        List<ITagable> Children { get; set; }
        public string TagColor { get; set; }
        public SolidColorBrush TagFontColor { get; }

    }
}