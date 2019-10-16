using System.Windows;
using Asset_Management_System.Logging;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for DialogBoxWithInput.xaml
    /// </summary>
    public partial class ShowEntry : Window
    {
        public ShowEntry(Entry entry)
        {
            InitializeComponent();
            DataContext = this;
            this.LabelText = "Log entry: " + entry.Id.ToString();
            this.DescriptionText = entry.Description;
            this.UserText = "By: " + entry.Username;
            this.TimeText = "At: " + entry.DateToStringConverter;
            this.OptionsText = RemoveUnwantedChars(entry.Options);
        }

        public string LabelText
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public string DescriptionText
        {
            get => Description.Text;
            set => Description.Text = value;
        }

        public string UserText
        {
            get => User.Text;
            set => User.Text = value;
        }
        
        public string OptionsText
        {
            get => Options.Text;
            set => Options.Text = value;
        }

        private string RemoveUnwantedChars(string options)
        {
            return options
                .Replace('{', '\n')
                .Replace('}', '\0')
                .Replace('\\', '\0')
                .Replace(',', '\n')
                .Replace('"', '\0');
        }
        
        public string TimeText
        {
            get => Time.Text;
            set => Time.Text = value;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        
    }
}