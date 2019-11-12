using System.Collections.Generic;

namespace Asset_Management_System.Models
{
    public interface ITagable
    {
        ulong TagId { get; }
        string TagType { get; }
        string TagLabel { get; }
        List<ITagable> Children { get; set; }
    }
}