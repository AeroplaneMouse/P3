using System.Windows.Controls;
using AMS.Events;

namespace AMS.Views.Prompts
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : Page
    {
        public TextInput(string message, PromptEventHandler handler)
            : this(message, null, handler) { }

        public TextInput(string message, string startingText, PromptEventHandler handler )
        {
            InitializeComponent();
            DataContext = new ViewModels.Prompts.TextInputViewModel(message, startingText, handler);
        }
    }
}
