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

        public PromtForReportName(string input, string label)
        {
            InitializeComponent();
            DataContext = this;
            this.InputText = input;
            this.LabelText = label;
        }

        public string InputText {
            get { return Input.Text; }
            set { Input.Text = value; }
        }

        public string LabelText {
            get { return Label.Text; }
            set { Label.Text = value; }
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
