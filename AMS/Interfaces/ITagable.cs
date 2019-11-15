using System.Collections.Generic;

namespace AMS.Models
{
    public interface ITagable
    {
        ulong TagId { get; }
        string TagType { get; }
        string TagLabel { get; }
        List<ITagable> Children { get; set; }
    }
}