namespace AMS.ConfigurationHandler
{
    public interface IConFigurationHandler
    {
        string GetConfigValue();
        void SetConfigValue(string newValue);
        
    }
}