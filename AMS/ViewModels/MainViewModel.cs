using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using AMS.Models;
using AMS.Events;
using AMS.Database;
using AMS.Authentication;
using AMS.Views;
using AMS.Database.Repositories;

namespace AMS.ViewModels
{
    public class MainViewModel : Base.BaseViewModel
    {
        // The window this view model controls
        private readonly Window _window;

        // Margin around the window to allow a drop shadow
        private int _outerMarginSize;

        private Department _currentDepartment;
        private bool _hasConnectionFailedBeenRaised = false;

        public double WindowMinWidth { get; set; }
        public double WindowMinHeight { get; set; }
        public int InnerContentPaddingSize { get; set; }
        public Thickness InnerContentPadding { get => new Thickness(0); }
        public int ResizeBorder { get; set; }
        public Thickness ResizeBorderThickness { get => new Thickness(ResizeBorder + OuterMarginSize); }
        public int OuterMarginSize
        {
            // If the window is maximised, remove the margin around the window, we don't need the drop shadow
            get => _window.WindowState == WindowState.Maximized ? 0 : _outerMarginSize;
            set => _outerMarginSize = value;
        }
        public Thickness OuterMarginThicknessSize { get => new Thickness(OuterMarginSize); }
        public int TitleHeight { get; set; }
        public int NavigationHeight { get; set; }
        public GridLength TitleHeightGridLength { get => new GridLength(TitleHeight + ResizeBorder); }
        public String CurrentUser { get; set; }
        public Department CurrentDepartment
        {
            get => _currentDepartment;
            set
            {
                _currentDepartment = value;
                OnPropertyChanged(nameof(CurrentDepartment));

                // Update default department for user
                if (_currentDepartment != null)
                    CurrentSession.user.DefaultDepartment = _currentDepartment.ID;
            }
        }
        public Frame ContentFrame { get; set; } = new Frame();
        public Page SplashPage { get; set; }
        public Page PopupPage { get; set; }
        //public Visibility SplashVisibility { get; set; } = Visibility.Visible;
        public Visibility CurrentDepartmentVisibility { get; set; } = Visibility.Hidden;
        public Visibility Visible { get; set; }
        public List<Department> Departments { get => GetDepartments(); }
        public Session CurrentSession { get; private set; }
        public ObservableCollection<Notification> ActiveNotifications { get; private set; } = new ObservableCollection<Notification>();


        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel(Window window)
        {
            // Setting private fields
            _window = window;
            _outerMarginSize = 10;

            WindowMinWidth = 300;
            WindowMinHeight = 400;

            ResizeBorder = 4;
            TitleHeight = 25;
            InnerContentPaddingSize = 6;

            // Listen out for the window resizing
            _window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginThicknessSize));
            };

            // Initialize commands
            MinimizeCommand = new Base.RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new Base.RelayCommand(() => _window.WindowState ^= WindowState.Maximized); // Changes between normal and maximized
            CloseCommand = new Base.RelayCommand(() => _window.Close());

            SystemMenuCommand = new Base.RelayCommand(() => SystemCommands.ShowSystemMenu(_window, GetMousePosition()));

            ShowHomePageCommand = new Base.RelayCommand(() => ContentFrame.Navigate(new Home()));
            //ShowAssetsPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Assets(this, _assetService)));
            //ShowTagPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Tags(this, _tagService)));
            //ShowLogPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Logs(this, _entryService)));

            //RemoveNotificationCommand = new Commands.RemoveNotificationCommand(this);

            ReloadCommand = new Base.RelayCommand(Reload);

            //AddFieldTestCommand = new Base.RelayCommand();

            ImportUsersCommand = new Base.RelayCommand(ImportUsers);

            SelectDepartmentCommand = new Base.RelayCommand<object>(SelectDepartment);
            //RemoveDepartmentCommand = new Commands.RemoveDepartmentCommand(this, _departmentService);
            //EditDepartmentCommand = new Commands.EditDepartmentCommand(this, _departmentService);
            AddDepartmentCommand = new Base.RelayCommand(() =>
            {
                DisplayPrompt(new Views.Prompts.TextInput("Enter the name of your new department", AddDepartment));
            });

            // Fixes window sizing issues at maximized
            var resizer = new Resources.Window.WindowResizer(_window);

            // Display splash page
            SplashPage = new Views.Splash(this);
        }

        private void SelectDepartment(object parameter)
        {
            try
            {
                ulong id = ulong.Parse(parameter.ToString());
                Department selectedDepartment = new DepartmentRepository().GetById(id);
                if (selectedDepartment == null)
                    selectedDepartment = Models.Department.GetDefault();

                AddNotification(new Models.Notification(
                    $"{selectedDepartment.Name} is now the current department.", Models.Notification.APPROVE));
                CurrentDepartment = selectedDepartment;
            }
            catch (Exception e)
            {
                AddNotification(new Models.Notification(e.Message, Models.Notification.ERROR), 5000);
            }
        }

        public void DisplayPrompt(Page promptPage)
        {
            PopupPage = promptPage;
            (promptPage.DataContext as Prompts.PromptViewModel).PromptElapsed += PromptElapsed;
        }

        private void PromptElapsed(object sender, PromptEventArgs e)
        {
            // Removing popup
            PopupPage = null;
        }

        public void AddNotification(string message, SolidColorBrush foreground, SolidColorBrush background)
            => AddNotification(new Notification(message, foreground, background));

        public void AddNotification(Notification n) => AddNotification(n, 2500);

        public async void AddNotification(Notification n, int displayTime)
        {
            // Add notification to the list of active notifications
            ActiveNotifications.Add(n);

            // Wait some time, then remove it.
            if (displayTime > 0)
            {
                await Task.Delay(displayTime);
                RemoveNotification(n);
            }
        }

        // Removes an active notification.
        public void RemoveNotification(int id) =>
            RemoveNotification(ActiveNotifications.Where(n => n.ID == id).First());

        public void RemoveNotification(Notification n)
        {
            ActiveNotifications.Remove(n);
        }


        /// <summary>
        /// Used when the application has connected to the database and other external services,
        /// to remove the splash page and shows the navigation menu's and homepage.
        /// </summary>
        //public void SystemLoaded(object _session, EventArgs e)
        public void LoadSystem(Session session)
        {
            // Attaching notification
            MySqlHandler.ConnectionFailed += Reload;

            // Loads homepage and other stuff from the UI-thread.
            SplashPage.Dispatcher.Invoke(Load);

            // Remove splash page
            SplashPage = null;

            // Resetting connection failed
            _hasConnectionFailedBeenRaised = false;

            // Show department and username
            CurrentDepartmentVisibility = Visibility.Visible;
            CurrentSession = session;
            CurrentUser = CurrentSession.Username;
            OnPropertyChanged(nameof(CurrentUser));

            // Setting the current department, from the default department of the current user.
            CurrentDepartment = new DepartmentRepository().GetById(session.user.DefaultDepartment);
            if (CurrentDepartment == null)
                CurrentDepartment = Department.GetDefault();
        }


        //private void ConnectionFailed(Notification n, bool reloadRequired)
        //{
        //    if (!HasConnectionFailedBeenRaised)
        //    {
        //        HasConnectionFailedBeenRaised = true;
        //        // Display notification if one was given
        //        if (n != null)
        //            AddNotification(n, 6000);

        //        // Reload the application is that is required
        //        if (reloadRequired)
        //            Reload();
        //    }
        //}

        // Loads excluded pages and sets homepage.
        private void Load()
        {
            // Load homepage
            ContentFrame.Navigate(new Home());
        }

        private void ImportUsers()
        {
            //ContentFrame.Navigate(new Views.UserImporterView(this, _userService, _departmentService));
        }

        // Used to reload the application
        private void Reload()
        {
            Console.WriteLine("Reloading...");

            // Clearing memory
            MySqlHandler.ConnectionFailed -= Reload;
            CurrentDepartmentVisibility = Visibility.Hidden;
            CurrentUser = null;
            CurrentDepartment = null;
            OnPropertyChanged(nameof(CurrentUser));

            // Load splash screen
            SplashPage = new Views.Splash(this);
        }

        private List<Department> GetDepartments()
        {
            if (CurrentDepartmentVisibility == Visibility.Visible)
                return (List<Department>)new DepartmentRepository().GetAll();
            else
                return new List<Department>();
        }

        private void AddDepartment(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                Department department = new Department();
                department.Name = e.ResultMessage;

                ulong id;
                if (new DepartmentRepository().Insert(department, out id))
                {
                    // TODO: Add log of department insert
                    OnPropertyChanged(nameof(Departments));
                    AddNotification(new Notification($"{department.Name} has now been add to the system.",
                        Notification.APPROVE));
                }
                else
                    AddNotification(
                        new Notification(
                            $"ERROR! An unknown error stopped the department {department.Name} from beeing added.",
                            Notification.ERROR), 3000);
            }
        }

        // Window commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SystemMenuCommand { get; set; }

        public ICommand ShowHomePageCommand { get; set; }
        public ICommand ShowAssetsPageCommand { get; set; }
        public ICommand ShowTagPageCommand { get; set; }

        // Department commands
        public static ICommand SelectDepartmentCommand { get; set; }
        public static ICommand RemoveDepartmentCommand { get; set; }
        public static ICommand EditDepartmentCommand { get; set; }
        public ICommand AddDepartmentCommand { get; set; }

        public ICommand ShowLogPageCommand { get; set; }

        public ICommand ReloadCommand { get; set; }

        // Notification commands
        public ICommand AddFieldTestCommand { get; set; }
        public static ICommand RemoveNotificationCommand { get; set; }
        public ICommand ImportUsersCommand { get; set; }



        #region Magic from StackOverflow: https://stackoverflow.com/questions/4226740/how-do-i-get-the-current-mouse-screen-coordinates-in-wpf

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        // Gets the current mouse position on the screen
        private static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        #endregion
    }
}