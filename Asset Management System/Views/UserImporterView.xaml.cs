using System.Windows;
﻿using Asset_Management_System.ViewModels;
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
using System.Windows.Shapes;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for UserImporterView.xaml
    /// </summary>
    public partial class UserImporterView : Page
    {
        public UserImporterView(MainViewModel main, IUserService userService, IDepartmentService departmentService)
        {
            InitializeComponent();
            this.DataContext = new UserImporterViewModel(main, userService, departmentService);
        }
    }
}
