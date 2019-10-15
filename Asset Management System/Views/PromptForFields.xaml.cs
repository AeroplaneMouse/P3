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
    public partial class PromptForFields : Window
    {
        public PromptForFields(string label)
        {
            InitializeComponent();
            DataContext = this;
            this.LabelText = label;
        }

        public bool Required
        {
            get { return FieldRequired.IsEnabled; }
        }

        public string FieldName
        {
            get { return Input.Text; }
            set { Input.Text = value; }
        }

        public string LabelText
        {
            get { return Label.Text; }
            set { Label.Text = value; }
        }

        public string DefaultValueText
        {
            get { return DefaultValue.Text; }
            set { DefaultValue.Text = value; }
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