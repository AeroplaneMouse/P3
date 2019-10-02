using System;
using System.Windows;
using System.Windows.Controls;
using Asset_Management_System.Database;
using Asset_Management_System.Events;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for Assets.xaml
    /// </summary>
    public partial class Assets : Page
    {
        public static event ChangeSourceEventHandler ChangeSourceRequest;

        public Assets()
        {
            InitializeComponent();
            //DBConnection db = DBConnection.Instance();
            //if(db.IsConnect())
            //{

            //}
        }

        private void Btn_AddNewAsset_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeSourceRequest != null)
                ChangeSourceRequest?.Invoke(this, new ChangeSourceEventArgs(new NewAsset()));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load assets from database
        }
    }
}
