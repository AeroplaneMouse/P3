using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands.ViewModelHelper;

namespace Asset_Management_System.Views
{
    public abstract class FieldsController : Page
    {
        public ObservableCollection<Field> FieldsList { get; set; }
        protected bool _editing;
    }
}