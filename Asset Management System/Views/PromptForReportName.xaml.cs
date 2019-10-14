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
using System.Windows.Shapes;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for DialogBoxWithInput.xaml
    /// </summary>
    public partial class PromtForReportName : Window
    {

        public PromtForReportName()
        {
            InitializeComponent();
            DataContext = this;
            this.ResponseText = "asset_report_" + DateTime.Now.ToString().Replace(@"/", "").Replace(@" ", "-").Replace(@":", "") + ".csv";
        }

        public string ResponseText {
            get { return Input.Text; }
            set { Input.Text = value; }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
