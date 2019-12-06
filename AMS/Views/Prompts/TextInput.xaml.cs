using System.Windows.Controls;
using AMS.Events;
using AMS.ViewModels.Prompts;

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
            DataContext = new TextInputViewModel(message, startingText, handler);
        }
    }
}
