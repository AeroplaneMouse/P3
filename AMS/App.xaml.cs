using AMS.ViewModels;
using System;
using System.Windows;

namespace AMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs args)
        {
            Window main = Features.Create.MainWindow();
            main.Show();
            return;
            try
            {
                
            }
            catch(Exception e)
            {
                MessageBox.Show($"An unknown process error occured:{Environment.NewLine}{e}", "Unknown process error!");
                throw;
            }
        }
    }
}
