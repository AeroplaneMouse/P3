namespace AMS.ConfigurationHandler
{
    public interface IConFigurationHandler
    {
        string GetConfigValue(out bool exists);
        void SetConfigValue(string newValue);
        
    }
}