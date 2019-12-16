using System.Windows.Controls;
using AMS.Events;
using AMS.Models;
using AMS.ViewModels.Prompts;

namespace AMS.Views.Prompts
{
    public partial class CustomFunction : Page
    {
        public CustomFunction(string message, PromptEventHandler handler, Function inputField = null)
        {
            InitializeComponent();
            DataContext = new CustomFunctionViewModel(message, handler, inputField);
        }
    }
}