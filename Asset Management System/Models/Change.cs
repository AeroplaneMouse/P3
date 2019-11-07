namespace Asset_Management_System.Models
{
    public class Change
    {
        public string PrevValue { get; set; }
        public string NewValue { get; set; }

        /// <summary>
        /// Default Constructor
        ///
        /// Stores the new value and the old value as strings
        /// </summary>
        /// <param name="prevValue"></param>
        /// <param name="newValue"></param>
        public Change(string prevValue, string newValue)
        {
            PrevValue = prevValue;
            NewValue = newValue;
        }
    }
}