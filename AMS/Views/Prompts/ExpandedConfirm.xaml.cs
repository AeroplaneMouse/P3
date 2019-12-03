using AMS.Events;
using System.Windows.Controls;
using System.Collections.Generic;

namespace AMS.Views.Prompts
{
    /// <summary>
    /// Interaction logic for ExpandedConfirm.xaml
    /// </summary>
    public partial class ExpandedConfirm : Page
    {
        public ExpandedConfirm(string message, List<string> buttons, PromptEventHandler handler)
        {
            InitializeComponent();
            DataContext = new ViewModels.Prompts.ExpandedConfirmViewModel(message, buttons, handler);
        }
    }
}
