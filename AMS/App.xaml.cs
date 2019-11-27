using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AMS.Helpers;
using Features = AMS.Helpers.Features.Features;

namespace AMS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            Window main = Features.Instance.Create.MainWindow();
            main.Show();
        }
    }
}
