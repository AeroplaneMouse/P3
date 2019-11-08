namespace Asset_Management_System.Models
{
    public class Tagged
    {
        public ulong AssetId { get; set; }
        public ulong TagableId { get; set; }
        public string TagableType { get; set; }
    }
}