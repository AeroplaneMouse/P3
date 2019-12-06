using System.Windows.Controls;
using AMS.Events;
using AMS.Models;
using AMS.ViewModels.Prompts;

namespace AMS.Views.Prompts
{
    /// <summary>
    /// Interaction logic for CustomField.xaml
    /// </summary>
    public partial class CustomField : Page
    {
        public CustomField(string message, PromptEventHandler handler, bool isCustom = false, Field inputField = null)
        {
            InitializeComponent();
            DataContext = new CustomFieldViewModel(message, handler, isCustom, inputField);
        }
    }
}
