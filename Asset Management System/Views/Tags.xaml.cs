﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : FieldsController
    {
        public Tags(MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.TagsViewModel(main, ListPageType.Tag);
        }
    }
}