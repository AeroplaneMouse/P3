using System.Windows.Controls;
using AMS.Events;
using AMS.ViewModels.Prompts;

namespace AMS.Views.Prompts
{
    /// <summary>
    /// Interaction logic for Confirm.xaml
    /// </summary>
    public partial class Confirm : Page
    {
        public Confirm(string message, PromptEventHandler handler)
        {
            InitializeComponent();
            DataContext = new ConfirmViewModel(message, handler);
        }
    }
}
