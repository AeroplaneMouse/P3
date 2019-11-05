namespace Asset_Management_System.Models
{
    public interface ITagable
    {
        ulong TagId();
        string TagType();
        string TagLabel();
    }
}