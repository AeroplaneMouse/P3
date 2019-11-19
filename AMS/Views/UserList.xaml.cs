using AMS.Controllers.Interfaces;
using AMS.ViewModels;
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

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserList : Page
    {
        public UserList(MainViewModel main, IUserListController controller)
        {
            InitializeComponent();
            this.DataContext = new UserListViewModel(main, controller);
        }
    }
}
