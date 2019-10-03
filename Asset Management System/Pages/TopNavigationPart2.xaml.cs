using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Asset_Management_System;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using System.Linq;

namespace Asset_Management_System.Pages
{
    /// <summary>
    /// Interaction logic for TopNavigation.xaml
    /// </summary>
    public partial class TopNavigationPart2 : Page
    {
        public event ChangeSourceEventHandler ChangeSourceRequest;
        public event ChangeFrameModeEventHandler ExpandFrameRequest;

        private const string Expand = "Expand";
        private const string Collapse = "Collapse";
        private MainWindow Main;
        private Department SelectedDepartment;


        public TopNavigationPart2(MainWindow main)
        {
            InitializeComponent();
            Main = main;
            Session session = new Session();
            LblCurrentUser.Content = session.Username;
        }

        private void ChangeDepartmentVisuals(string newState)
        {
            if (newState == Expand)
            {
                // Make the suggestion list visible
                LbDepartments.Visibility = Visibility.Visible;
                BtnShowDepartments.Background = Brushes.White;
                BtnShowDepartments.Foreground = Brushes.Black;

                ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Extend, ChangeFrameModeEventArgs.Down);
                if (ExpandFrameRequest != null)
                    ExpandFrameRequest?.Invoke(this, args);
            }
            else if (newState == Collapse)
            {
                // Hide the suggestion list
                LbDepartments.Visibility = Visibility.Hidden;
                BtnShowDepartments.Background = Brushes.Transparent;
                BtnShowDepartments.Foreground = Brushes.White;

                // Collapse the navigation frame
                ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Collapse, ChangeFrameModeEventArgs.Up);
                if (ExpandFrameRequest != null)
                    ExpandFrameRequest?.Invoke(this, args);
            }
            else
                throw new ArgumentException();
        }

        private void BtnShowDepartments_Click(object sender, RoutedEventArgs e)
        {
            if (LbDepartments.Visibility == Visibility.Hidden)
            {
                ChangeDepartmentVisuals(Expand);

                // Fill suggestion list
                DepartmentRepository dep = new DepartmentRepository();
                List<Department> departments = dep.GetAll();
                //List<string> department_names = departments.Select(s => s.Name).ToList();

                //LbDepartments.ItemsSource = department_names;

                //List<Department> testDepartments = new List<Department>();
                //testDepartments.Add(new Department("IT"));
                //testDepartments.Add(new Department("HR"));
                //testDepartments.Add(new Department("Finance"));
                //testDepartments.Add(new Department("Zookeeper"));

                // Adding department items to list of department
                List<Grid> testElements = new List<Grid>();
                foreach (Department department in departments)
                    testElements.Add(GenerateBlockElement(department));

                LbDepartments.ItemsSource = testElements;
            }
            else
            {
                ChangeDepartmentVisuals(Collapse);
                LbDepartments.ItemsSource = null;
                LbDepartments.Items.Clear();
            }
        }

        private Grid GenerateBlockElement(Department department)
        {
            Grid grid = new Grid();
            ColumnDefinition c0 = new ColumnDefinition();
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            c0.Width = new GridLength(10, GridUnitType.Star);
            c1.Width = new GridLength(30, GridUnitType.Pixel);
            c2.Width = new GridLength(30, GridUnitType.Pixel);
            grid.ColumnDefinitions.Add(c0);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);

            // Creating item
            Label item = new Label() {
                Content = department,
                Width = 150
            };
            Grid.SetColumn(item, 0);

            // Creating pencil
            //Image img = new Image();
            //Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly().GetName());
            //img.Source = new BitmapImage(new Uri(@"pack://application:,,,/Asset Management System;component/Resources/pencil.png"));
            
            Button pencil = new Button() { 
                Content= "E",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(5)
            };
            pencil.Click += BtnEdit_Click;
            Grid.SetColumn(pencil, 1);

            // Creating trashcan
            Button trash = new Button() { 
                Content= "R",
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(5)
            };
            trash.Click += BtnRemove_Click;
            Grid.SetColumn(trash, 2);

            grid.Children.Add(item);
            grid.Children.Add(pencil);
            grid.Children.Add(trash);

            return grid;
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            SelectedDepartment = GetDeparment(sender);

            // Generate popup window
            Popup page = new Popup();
            page.LbInfoLine1.Content = $"Are you sure that you want";
            page.LbInfoLine2.Content = $"to DELETE department";
            page.LbInfoLine3.Content = $"{ SelectedDepartment.Name }";
            page.BtnYes.Click += DeleteDepartment;
            page.BtnNo.Click += RemovePopup;
            ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Extend, ChangeFrameModeEventArgs.Down, Main.FramePopup);
            Main.FramePopup.Content = page;

            if (ExpandFrameRequest != null)
                ExpandFrameRequest?.Invoke(this, args);
        }

        private void DeleteDepartment(object sender, RoutedEventArgs e)
        {
            RemovePopup();

            // Delete the department from the database
            throw new NotImplementedException("The removal of departments has not yet been implemented.");
        }

        private void RemovePopup() => RemovePopup(null, null);
        private void RemovePopup(object sender, RoutedEventArgs e)
        {
            Main.FramePopup.Content = null;
            ChangeFrameModeEventArgs args = new ChangeFrameModeEventArgs(ChangeFrameModeEventArgs.Collapse, ChangeFrameModeEventArgs.Up, Main.FramePopup);
        }

        private Department GetDeparment(object sender)
        {
            Grid grid = VisualTreeHelper.GetParent(sender as Button) as Grid;
            Label item = grid.Children[0] as Label;
            return (Department)item.Content;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            ChangeDepartmentVisuals(Collapse);
            try
            {
                Page page = new EditDepartment(GetDeparment(sender));

                if (ChangeSourceRequest != null)
                    ChangeSourceRequest?.Invoke(this, new ChangeSourceEventArgs(page));
            }
            catch(Exception f)
            {
                Console.WriteLine($"There was an error when rerouting to the edit department page: { f }");
            }


            //UIElement item = sender as Button;
            //for (int i = 0; i < 4; i++)
            //{
            //    item = VisualTreeHelper.GetParent(item) as UIElement;
            //}

            //LbDepartments.SelectedItem = item as ListBoxItem;

            //if (ChangeSourceRequest != null)
            //    ChangeSourceRequest?.Invoke(this, e);
        }
    }
}
