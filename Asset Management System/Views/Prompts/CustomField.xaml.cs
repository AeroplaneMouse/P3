using System.Windows.Controls;
using Asset_Management_System.Events;

namespace Asset_Management_System.Views.Prompts
{
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : Page
    {
        public CustomField(string message, PromptEventHandler handler,bool isCustom = false)
        {
            InitializeComponent();
            DataContext = new ViewModels.Prompts.CustomFieldViewModel(message, handler,isCustom);
        }
    }
}
