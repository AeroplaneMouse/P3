using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Asset_Management_System.Views.Prompts
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
