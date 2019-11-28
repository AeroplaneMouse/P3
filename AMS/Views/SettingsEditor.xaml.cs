using System.Windows.Controls;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : Page
    {
        public SettingsEditor()
        {
            InitializeComponent();
            DataContext = new ViewModels.SettingsEditorViewModel();
        }
    }
}
