using System;
using AMS.Views;
using AMS.Models;
using AMS.Events;
using System.Linq;
using AMS.Database;
using System.Windows;
using AMS.Views.Prompts;
using AMS.Authentication;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using AMS.Database.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using AMS.Controllers;
using AMS.Database.Repositories.Interfaces;
using AMS.Helpers;
using AMS.IO;
using System.Windows.Navigation;
using AMS.ConfigurationHandler;

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

        private IUserRepository _userRep;
        private IDepartmentRepository _departmentRep;

        #region Window Properties
        
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
        public GridLength TitleHeightGridLength { get => new GridLength(TitleHeight + ResizeBorder); }
        public int NavigationHeight { get; set; }

        #endregion

        public Stack<Page> History = new Stack<Page>();

        public string CurrentUser { get; set; }
        public string CurrentDatabase { get; set; }
        public Session CurrentSession { get; private set; }
        public Department CurrentDepartment
        {
            get => _currentDepartment;
            set
            {
                _currentDepartment = value;

                // Update default department for user
                if (_currentDepartment != null)
                {
                    CurrentSession.user.DefaultDepartment = _currentDepartment.ID;
                    _userRep.Update(CurrentSession.user);
                }

                OnPropertyChanged(nameof(CurrentDepartment));
            }
        }

        public Frame ContentFrame { get; set; } = new Frame();
        public Page SplashPage { get; set; }
        public Page PopupPage { get; set; }

        public Visibility CurrentDepartmentVisibility { get; set; } = Visibility.Hidden;
        public Visibility OnlyVisibleForAdmin { get; private set; } = Visibility.Collapsed;

        public List<Department> Departments { get => GetDepartments(); }
        public ObservableCollection<Notification> ActiveNotifications { get; private set; } = new ObservableCollection<Notification>();

        // Window commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SystemMenuCommand { get; set; }

        // Change page commands
        public ICommand ShowHomePageCommand { get; set; }
        public ICommand ShowAssetListPageCommand { get; set; }
        public ICommand ShowTagListPageCommand { get; set; }
        public ICommand ShowLogPageCommand { get; set; }
        public ICommand ShowUserListPageCommand { get; set; }

        // Department commands
        public ICommand SelectDepartmentCommand { get; set; }
        public ICommand RemoveDepartmentCommand { get; set; }
        public ICommand EditDepartmentCommand { get; set; }
        public ICommand AddDepartmentCommand { get; set; }

        // Notification commands
        public ICommand RemoveNotificationCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand ShowShortcutsCommand { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel(Window window, IUserRepository userRepository, IDepartmentRepository departmentRepository)
        {
            InitializeWindowsCommands();
            
            //FileConfigurationHandler _configurationhandler = new FileConfigurationHandler(CurrentSession);
            //_configurationhandler.SetConfigValue("Server=192.38.49.9; database=ds303e19_utf16; UID=ds303e19; password=Cisptf8CuT4hLj4T; Charset=utf8; Connect Timeout=5");
            
            // Setting private fields
            _window = window;
            _outerMarginSize = 10;

            WindowMinWidth = 300;
            WindowMinHeight = 400;

            ResizeBorder = 4;
            TitleHeight = 26;
            InnerContentPaddingSize = 6;

            // Listen out for the window resizing
            _window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginThicknessSize));
            };

            // Fixes window sizing issues at maximized
            var resizer = new Resources.Window.WindowResizer(_window);

            Features.Main = this;

            _userRep = userRepository;
            _departmentRep = departmentRepository;

            SplashPage = Features.Create.Splash(this);
        }

        public void LoadSystem(Session session)
        {
            // Attaching notification
            MySqlHandler.ConnectionFailed += ConnectionFailed;

            // Loads homepage and other stuff from the UI-thread.
            SplashPage.Dispatcher.Invoke(() => ContentFrame.Navigate(Features.Create.Home()));

            // Remove splash page
            SplashPage = null;

            // Resetting connection failed
            _hasConnectionFailedBeenRaised = false;

            // Show department and username
            CurrentDepartmentVisibility = Visibility.Visible;
            CurrentSession = session;
            CurrentDatabase = new MySqlHandler().GetDatabaseName();
            CurrentUser = CurrentSession.Username;
            OnPropertyChanged(nameof(CurrentUser));

            // Sets the visibility of WPF elements binding to this, based on whether or not the current user is an admin
            OnlyVisibleForAdmin = CurrentSession.IsAdmin() ? Visibility.Visible : Visibility.Collapsed;

            // Setting the current department, from the default department of the current user.
            CurrentDepartment = _departmentRep.GetById(session.user.DefaultDepartment);

            if (CurrentDepartment == null)
                CurrentDepartment = Department.GetDefault();

            InitializeCommands();
        }

        /// <summary>
        /// Displays the given prompt and attaches a remove delegate to remove it
        /// after accept or cancel.
        /// </summary>
        public void DisplayPrompt(Page promptPage)
        {
            PopupPage = promptPage;
            (promptPage.DataContext as Prompts.PromptViewModel).PromptElapsed += RemovePrompt;
        }

        /// <summary>
        /// Adds a notification to the list of active notifications, and removes is after the specified displayTime.
        /// If the displayTime is 0, the notification will not be removed.
        /// </summary>
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

        /// <summary>
        /// Adds a notification to the list of active notifications, with a displayTime of 2500 milliseconds.
        /// </summary>
        public void AddNotification(Notification n) => AddNotification(n, 2500);

        /// <summary>
        /// Used when the application has connected to the database and other external services,
        /// to remove the splash page and shows the navigation menu's and homepage.
        /// </summary>
        

        private void InitializeCommands()
        {
            RemoveNotificationCommand = new Base.RelayCommand<object>((parameter) => RemoveNotification(parameter as Notification));
            ReloadCommand = new Base.RelayCommand(Reload);
            ShowShortcutsCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.ShortcutsList()));

            // Change page commands
            ShowHomePageCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.Home()));
            ShowAssetListPageCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.AssetList()));
            ShowTagListPageCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.TagList()), () => Features.GetCurrentSession().IsAdmin());
            ShowLogPageCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.LogList()), () => Features.GetCurrentSession().IsAdmin());
            ShowUserListPageCommand = new Base.RelayCommand(() => Features.Navigate.To(Features.Create.UserList()), () => Features.GetCurrentSession().IsAdmin());

            // Department commands
            SelectDepartmentCommand = new Base.RelayCommand<object>(SelectDepartment);
            RemoveDepartmentCommand = new Commands.RemoveDepartmentCommand(this, () => Features.GetCurrentSession().IsAdmin());
            EditDepartmentCommand = new Commands.EditDepartmentCommand(this, () => Features.GetCurrentSession().IsAdmin());
            AddDepartmentCommand = new Base.RelayCommand(() => DisplayPrompt(new TextInput("Enter the name of your new department", AddDepartment)), () => Features.GetCurrentSession().IsAdmin());
        }

        private void InitializeWindowsCommands()
        {
            // Window commands
            MinimizeCommand = new Base.RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new Base.RelayCommand(() => _window.WindowState ^= WindowState.Maximized); // Changes between normal and maximized
            CloseCommand = new Base.RelayCommand(() => _window.Close());
            SystemMenuCommand = new Base.RelayCommand(() => SystemCommands.ShowSystemMenu(_window, _window.PointToScreen(
                new Point(
                    OuterMarginSize,
                    OuterMarginSize + ResizeBorder + TitleHeight
                )
            )));
        }

        /// <summary>
        /// Removes the prompt view
        /// </summary>
        private void RemovePrompt(object sender, PromptEventArgs e) => PopupPage = null;

        /// <summary>
        /// Removes an active notification by notification object
        /// </summary>
        private bool RemoveNotification(Notification n) => ActiveNotifications.Remove(n);

        /// <summary>
        /// Calls the reload method, if it has not been called and sets a flag to prevent
        /// it from being called again if multiple connection failes occur within a short amount of time.
        /// </summary>
        private void ConnectionFailed()
        {
            if (!_hasConnectionFailedBeenRaised)
            {
                _hasConnectionFailedBeenRaised = true;
                Reload();
            }
        }

        /// <summary>
        /// Resets saved content, and reconnects to the database.
        /// </summary>
        private void Reload()
        {
            Console.WriteLine("Reloading...");

            // Clearing memory
            MySqlHandler.ConnectionFailed -= ConnectionFailed;
            CurrentDepartmentVisibility = Visibility.Hidden;
            CurrentUser = null;
            CurrentDepartment = null;
            OnPropertyChanged(nameof(CurrentUser));

            // Load splash screen
            SplashPage = Features.Create.Splash(this);
        }

        /// <summary>
        /// Selects a new department where parameter is the id of the department to be selected
        /// </summary>
        private void SelectDepartment(object parameter)
        {
            try
            {
                ulong id = ulong.Parse(parameter.ToString());
                Department selectedDepartment = _departmentRep.GetById(id);

                if (selectedDepartment == null)
                    selectedDepartment = Department.GetDefault();

                AddNotification(new Notification($"{selectedDepartment.Name} is now the current department.", Notification.APPROVE));

                CurrentDepartment = selectedDepartment;
            }
            catch (Exception e)
            {
                AddNotification(new Notification(e.Message, Notification.ERROR), 5000);
            }
        }

        /// <summary>
        /// Returns the list of deparments
        /// </summary>
        /// <returns></returns>
        private List<Department> GetDepartments()
        {
            if (CurrentDepartmentVisibility == Visibility.Visible)
                return (List<Department>)_departmentRep.GetAll();

            else
                return new List<Department>();
        }

        /// <summary>
        /// Adds a new department to the system if the given prompt resulted accept with the name specified
        /// </summary>
        private void AddDepartment(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                Department department = new Department();
                department.Name = (e as TextInputPromptEventArgs).Text;

                ulong id;
                if (_departmentRep.Insert(department, out id))
                {
                    // TODO: Add log of department insert
                    OnPropertyChanged(nameof(Departments));
                    AddNotification(new Notification($"{department.Name} has now been added to the system.",
                        Notification.APPROVE));
                }
                else
                    AddNotification(new Notification(
                            $"An unknown error stopped the department {department.Name} from beeing added.",
                            Notification.ERROR), 3000);
            }
        }
    }
}
