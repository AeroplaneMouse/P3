using System.Windows.Controls;
using AMS.Events;
using AMS.ViewModels.Prompts;

namespace AMS.Views.Prompts
{
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : Page
    {
        public CustomField(string message, PromptEventHandler handler, bool isCustom = false)
        {
            InitializeComponent();
            DataContext = new CustomFieldViewModel(message, handler, isCustom);
        }
    }
}
