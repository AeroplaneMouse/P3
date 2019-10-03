﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Events;
using Asset_Management_System.Pages;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Left_navigation.xaml
    /// </summary>
    public partial class LeftNavigation : Page
    {
        public event ChangeSourceEventHandler ChangeSourceRequest;
        private MainWindow Main;

        public LeftNavigation(MainWindow main)
        {
            InitializeComponent();
            Main = main;
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
            if(sender is Button btn)
            {
                Page page;
                page = btn.Name switch
                {
                    "Btn_homePage" => new Home(Main),
                    "Btn_assetsPage" => new Assets(),
                    "Btn_tagsPage" => new Tags(),
                    "Btn_settingsPage" => new Settings(),
                    "Btn_helpPage" => new Help(),
                    _ => null,
                };

                ChangeSourceRequest?.Invoke(this, new ChangeSourceEventArgs(page));
            }
        }
    }
}
