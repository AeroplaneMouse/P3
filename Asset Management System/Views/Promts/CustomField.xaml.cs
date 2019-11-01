using System.Windows.Controls;
using Asset_Management_System.Events;

namespace Asset_Management_System.Views.Promts
{
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : Page
    {
        public CustomField(string message, PromtEventHandler handler)
        {
            InitializeComponent();
            DataContext = new ViewModels.Promts.CustomFieldViewModel(message, handler);
        }
    }
}
