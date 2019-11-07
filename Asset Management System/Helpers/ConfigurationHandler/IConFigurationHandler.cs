namespace Asset_Management_System.Helpers.ConfigurationHandler
{
    public interface IConFigurationHandler
    {
        string GetConfigValue();
        void SetConfigValue(string newValue);
        
    }
}