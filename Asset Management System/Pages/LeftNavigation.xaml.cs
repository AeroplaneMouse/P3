﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Left_navigation.xaml
    /// </summary>
    public partial class LeftNavigation : Page
    {
        public event RoutedEventHandler ChangeSourceRequest;
        private readonly Session _currentSession;

        public LeftNavigation(Session session)
        {
            if (session != null)
                _currentSession = session;
            else
                throw new ArgumentNullException();

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
            if (ChangeSourceRequest != null)
                ChangeSourceRequest?.Invoke(this, e);
        }
    }
}
