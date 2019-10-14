namespace Asset_Management_System.Models
{
    public class Change
    {
        //public string PropertyName { get; set; }
        public string PrevValue { get; set; }
        public string NewValue { get; set; }

        public Change(string prevValue, string newValue)
        {
            //PropertyName = propertyName;
            PrevValue = prevValue;
            NewValue = newValue;
        }
    }
}