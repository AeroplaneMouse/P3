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

namespace Asset_Management_System.Views.Promts
{
    /// <summary>
    /// Interaction logic for Confirm.xaml
    /// </summary>
    public partial class Confirm : Page
    {
        public Confirm(string message, PromtEventHandler handler)
        {
            InitializeComponent();
            DataContext = new ViewModels.Promts.ConfirmViewModel(message, handler);
        }
    }
}
