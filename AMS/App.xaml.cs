using AMS.ViewModels;
using System.Windows;

namespace AMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Window main = Features.Create.MainWindow();
            main.Show();
        }
    }
}
