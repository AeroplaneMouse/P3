using System.Windows;

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
            get { return  (bool) FieldRequired.IsChecked; }
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