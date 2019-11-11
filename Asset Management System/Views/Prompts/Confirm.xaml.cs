using System.Windows.Controls;
using Asset_Management_System.Events;

namespace Asset_Management_System.Views.Prompts
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
