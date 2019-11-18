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

namespace AMS.ViewModels
{
    public class MainViewModel : Base.BaseViewModel
    {
        // The window this view model controls
        private Window _window;

        // Margin around the window to allow a drop shadow
        private int _outerMarginSize;

        private List<Page> pages = new List<Page>();
        private List<Page> excludedPages = new List<Page>();

        private Department _currentDepartment;
        private Stack<Page> _history = new Stack<Page>();
        private Page _currentPage;
        private bool HasConnectionFailedBeenRaised = false;

        //private IAssetService _assetService;
        //private ITagService _tagService;
        //private IEntryService _entryService;
        //private IUserService _userService;
        //private ICommentService _commentService;
        //private DepartmentService _departmentService;


        #region Constructor

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
                //OnPropertyChanged(nameof(WindowRadius));
                //OnPropertyChanged(nameof(WindowCornerRadius));
            };

            //TODO: Determine if this is composition root
            //_assetService = new AssetService(new AssetRepository());
            //_tagService = new TagService(new TagRepository());
            //_entryService = new EntryService(new LogRepository());
            //_userService = new UserService(new UserRepository());
            //_commentService = new CommentService(new CommentRepository());
            //_departmentService = new DepartmentService(new DepartmentRepository());

            // Setting up frames
            //SplashPage = new Views.Splash(this, _userService);

            // Initialize commands
            MinimizeCommand = new Base.RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new Base.RelayCommand(() => _window.WindowState ^= WindowState.Maximized); // Changes between normal and maximized
            CloseCommand = new Base.RelayCommand(() => _window.Close());

            SystemMenuCommand = new Base.RelayCommand(() => SystemCommands.ShowSystemMenu(_window, GetMousePosition()));


            //ShowHomePageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Home(this, _assetService, _tagService)));
            //ShowAssetsPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Assets(this, _assetService)));
            //ShowTagPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Tags(this, _tagService)));
            //ShowLogPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Logs(this, _entryService)));

            //RemoveNotificationCommand = new Commands.RemoveNotificationCommand(this);

            ReloadCommand = new Base.RelayCommand(Reload);

            //AddFieldTestCommand = new Base.RelayCommand();

            ImportUsersCommand = new Base.RelayCommand(ImportUsers);

            //SelectDepartmentCommand = new Commands.SelectDepartmentCommand(this, _departmentService);
            //RemoveDepartmentCommand = new Commands.RemoveDepartmentCommand(this, _departmentService);
            //EditDepartmentCommand = new Commands.EditDepartmentCommand(this, _departmentService);
            //AddDepartmentCommand = new Base.RelayCommand(() =>
            //{
            //    DisplayPrompt(new Views.Prompts.TextInput("Enter the name of your new department", AddDepartment));
            //});

            // Fixes window sizing issues at maximized
            var resizer = new Resources.Window.WindowResizer(_window);
        }

        #endregion


        #region Public Propterties

        // The smallest size the window can have 
        public double WindowMinWidth { get; set; }
        public double WindowMinHeight { get; set; }

        // Size of padding of the inner content of the main window
        public int InnerContentPaddingSize { get; set; }

        // Padding of the inner content of the main window
        public Thickness InnerContentPadding { get => new Thickness(0); }

        // Size of the resize border around the window
        public int ResizeBorder { get; set; }

        // The size of the resize border around the window, taking the outer margin into account
        public Thickness ResizeBorderThickness { get => new Thickness(ResizeBorder + OuterMarginSize); }

        // Margin around the window to allow a drop shadow. Checks if the window is maximised
        public int OuterMarginSize
        {
            // If the window is maximised, remove the margin around the window, we don't need the drop shadow
            get => _window.WindowState == WindowState.Maximized ? 0 : _outerMarginSize;
            set => _outerMarginSize = value;
        }

        // Thickness of the margin around the window to allow a drop shadow.
        public Thickness OuterMarginThicknessSize
        {
            get { return new Thickness(OuterMarginSize); }
        }

        // The height of the title bar / caption of the window
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
                {
                    CurrentSession.user.DefaultDepartment = _currentDepartment.ID;
                    //_userService.GetRepository().Update(CurrentSession.user);
                }
            }
        }

        public Frame ContentFrame { get; set; } = new Frame();

        public Page SplashPage { get; set; }

        public Page PopupPage { get; set; }

        //public Visibility SplashVisibility { get; set; } = Visibility.Visible;

        public List<Department> Departments
        {
            get => GetDepartments();
        }

        public Visibility CurrentDepartmentVisibility { get; set; } = Visibility.Hidden;

        public Session CurrentSession { get; private set; }

        //public ObservableCollection<Notification> ActiveNotifications { get; private set; } = new ObservableCollection<Notification>();

        public Visibility Visible { get; set; }

        // Accessed in that get main as parameter, dont know if its bad practice.
        //public IEntryService EntryService
        //{
        //    get => _entryService;
        //}

        #endregion

        #region Public Methods

        //public void DisplayPrompt(Page promptPage)
        //{
        //    PopupPage = promptPage;
        //    (promptPage.DataContext as Prompts.PromptViewModel).PromptElapsed += PromptElapsed;
        //}

        //private void PromptElapsed(object sender, PromptEventArgs e)
        //{
        //    // Removing popup
        //    PopupPage = null;
        //}


        /// <summary>
        /// Changes how many rows or columns a specific frame spans over. 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="dir"></param>
        //public void ChangeFrameExpasion(Frame frame, string dir)
        //{
        //    if (dir == FrameExpansionEventArgs.Right)
        //        Grid.SetColumnSpan(frame, 10);
        //    else if (dir == FrameExpansionEventArgs.Left)
        //        Grid.SetColumnSpan(frame, 1);
        //    else if (dir == FrameExpansionEventArgs.Down)
        //        Grid.SetRowSpan(frame, 10);
        //    else if (dir == FrameExpansionEventArgs.Up)
        //        Grid.SetRowSpan(frame, 1);
        //}

        /// <summary>
        /// Changes the content for the main content frame to the new page. If the page exists in the
        /// list of loaded pages, that one would be used. One can also specify a different frame of 
        /// which content will be modified to contain the new page.
        /// </summary>
        public void ChangeMainContent(Page newPage) => ChangeMainContent(newPage, ContentFrame);

        public void ChangeMainContent(Page newPage, Frame frame)
        {
            frame.Navigate(newPage);

            //Page setPage = null;
            //// Search the loaded page list, for the given page to check if it has allready been loaded.
            //foreach (Page page in pages)
            //{
            //    if (page.GetType() == newPage.GetType())
            //        setPage = page;
            //}

            //// If the new page wasn't found in the list, the given newPage object is used and added to the list of pages.
            //if (setPage == null)
            //{
            //    setPage = newPage;
            //    if (!ExcludedFromSaving(setPage))
            //        pages.Add(setPage);
            //}

            //// Update the list on the page, if there is one
            //if (setPage.DataContext is IListUpdate)
            //    (setPage.DataContext as IListUpdate).PageGotFocus();

            //if (!_history.Contains(_currentPage))
            //    _history.Push(_currentPage);
            //// Setting the content of the given frame, to the newPage object to display the requested page.
            //frame.Content = setPage;
            //_currentPage = setPage;
        }

        public void ReturnToPreviousPage()
        {
            ChangeMainContent(_history.Pop());
        }

        //public void AddNotification(string message, SolidColorBrush foreground, SolidColorBrush background)
        //    => AddNotification(new Notification(message, foreground, background));

        //public void AddNotification(Notification n) => AddNotification(n, 2500);

        //public async void AddNotification(Notification n, int displayTime)
        //{
        //    // Add notification to the list of active notifications
        //    ActiveNotifications.Add(n);

        //    // Wait some time, then remove it.
        //    if (displayTime > 0)
        //    {
        //        await Task.Delay(displayTime);
        //        RemoveNotification(n);
        //    }
        //}

        // Removes an active notification.
        //public void RemoveNotification(int id) =>
        //    RemoveNotification(ActiveNotifications.Where(n => n.ID == id).First());

        //public void RemoveNotification(Notification n)
        //{
        //    ActiveNotifications.Remove(n);
        //}


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
            HasConnectionFailedBeenRaised = false;

            // Show department and username
            CurrentDepartmentVisibility = Visibility.Visible;
            CurrentSession = session;
            CurrentUser = CurrentSession.Username;
            OnPropertyChanged(nameof(CurrentUser));

            // Setting the current department, from the default department of the current user.
            //CurrentDepartment = _departmentService.GetRepository().GetById(session.user.DefaultDepartment);
            //if (CurrentDepartment == null)
            //    CurrentDepartment = Department.GetDefault();
        }

        #endregion

        #region Private Methods

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
            // Add excluded pages
            //excludedPages.Add(new Views.AssetManager(null, _assetService));
            //excludedPages.Add(new Views.TagManager(null, _tagService));
            //excludedPages.Add(new Views.ObjectViewer(null, _commentService, null));
            //excludedPages.Add(new Views.UserImporterView(null, _userService, _departmentService));


            //// Load homepage
            //ChangeMainContent(new Views.Home(this, _assetService, _tagService));
        }

        private void ImportUsers()
        {
            //ChangeMainContent(new Views.UserImporterView(this, _userService, _departmentService));
        }

        // Used to reload the application
        private void Reload()
        {
            Console.WriteLine("Reloading...");

            //// Clearing memory
            //pages.Clear();
            //MySqlHandler.ConnectionFailed -= Reload;
            //CurrentDepartmentVisibility = Visibility.Hidden;
            //CurrentUser = null;
            //CurrentDepartment = null;
            //OnPropertyChanged(nameof(CurrentUser));

            //// Load splash screen
            //SplashPage = new Views.Splash(this, _userService);
        }

        private bool ExcludedFromSaving(Page page)
        {
            foreach (Page excludedPage in excludedPages)
            {
                // Return true if the page was found in the list of excluded pages.
                if (excludedPage.GetType() == page.GetType())
                    return true;
            }

            // If the page wasn't found, return false
            return false;
        }

        private List<Department> GetDepartments()
        {
            return null;
            //if (CurrentDepartmentVisibility == Visibility.Visible)
            //    return (List<Department>)((IDepartmentRepository)_departmentService.GetRepository()).GetAll();
            //else
            //    return new List<Department>();
        }

        //private void AddDepartment(object sender, PromptEventArgs e)
        //{
        //    if (e.Result)
        //    {
        //        Department department = new Department();
        //        department.Name = e.ResultMessage;

        //        ulong id;
        //        if (_departmentService.GetRepository().Insert(department, out id))
        //        {
        //            // TODO: Add log of department insert
        //            OnPropertyChanged(nameof(Departments));
        //            AddNotification(new Notification($"{department.Name} has now been add to the system.",
        //                Notification.APPROVE));
        //        }
        //        else
        //            AddNotification(
        //                new Notification(
        //                    $"ERROR! An unknown error stopped the department {department.Name} from beeing added.",
        //                    Notification.ERROR), 3000);
        //    }
        //}

        #endregion

        #region Commands

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


        #endregion

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