using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Events;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Left_navigation.xaml
    /// </summary>
    public partial class LeftNavigation : Page
    {
        public event ChangeSourceEventHandler ChangeSourceRequest;

        public LeftNavigation()
        {
            InitializeComponent();
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            Button but = sender as Button;
            but.BorderBrush = Brushes.White;
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Button but = sender as Button;
            but.BorderBrush = Brushes.Transparent;
        }

        private void Btn_OnClick(object sender, RoutedEventArgs e)
        {
            throw new ArgumentException();
            //if (ChangeSourceRequest != null)
            //    ChangeSourceRequest?.Invoke(this, );
        }
    }
}
