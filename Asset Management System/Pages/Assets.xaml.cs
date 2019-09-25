using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asset_Management_System
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        public static event EventHandler ChangeSourceRequest;

        public Assets()
        {
            InitializeComponent();

        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        {
            OnChangeSourceRequest(e);
        }

        void OnChangeSourceRequest(EventArgs e)
        {
            //EventHandler handler = ChangeSourceRequest;
            //if (handler != null)
            //    handler(this, e)
            ChangeSourceRequest?.Invoke(this, e);
        }
    }
}
