namespace Asset_Management_System.Helpers.ConfigurationHandler
{
    public interface IConFigurationHandler
    {
        string GetConfigValue(string key);
        string SetConfigValue(string key, string newValue);
        
    }
}