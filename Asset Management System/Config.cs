using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Asset_Management_System
{
    public static class Config
    {
        public static String GetApplicationSetting(String key){
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? null;
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return null;
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private static String GetRoamingConfigurationFile()
        {
            try
            {
                // Get the roaming configuration 
                // that applies to the current user.
                Configuration roamingConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
                return roamingConfig.FilePath;
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("[Exception error: {0}]",e.ToString());
                return null;
            }
        }
        /*
        public static void GetUserSetting(String key){

            try
            {
                var userSettings = ConfigurationManager.OpenExeConfiguration(GetRoamingConfigurationFile());
                string result = userSettings.AppSettings.Settings[key] ?? null;
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return null;
            }
        }

        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
        connectionStringsSection.ConnectionStrings["Blah"].ConnectionString = "Data Source=blah;Initial Catalog=blah;UID=blah;password=blah";
config.Save();
ConfigurationManager.RefreshSection("connectionStrings");
*/
    }
}
