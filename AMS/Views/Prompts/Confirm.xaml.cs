using System.Windows.Controls;
using AMS.Events;

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
            DataContext = new ViewModels.Prompts.ConfirmViewModel(message, handler);
        }
    }
}
