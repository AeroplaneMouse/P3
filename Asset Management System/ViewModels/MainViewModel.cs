using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Asset_Management_System.Models;
using Asset_Management_System.Authentication;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Events;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Asset_Management_System.ViewModels
{
    public class MainViewModel : Base.BaseViewModel
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel(Window window)
        {
            // Setting private fields
            _window = window;
            _outerMarginSize = 10;

            WindowMinWidth = 400;
            WindowMinHeight = 400;

            ResizeBorder = 4;
            TitleHeight = 25;
            InnerContentPaddingSize = 6;

            CurrentPage = DataModels.ApplicationPage.Start;

            // Listen out for the window resizeing
            _window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginThicknessSize));
                //OnPropertyChanged(nameof(WindowRadius));
                //OnPropertyChanged(nameof(WindowCornerRadius));
            };

            // Setting up frames
            Views.Splash splashScreen = new Views.Splash(this);
            FrameSplash.Content = splashScreen;

            // Initialize commands
            MinimizeCommand = new Base.RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new Base.RelayCommand(() => _window.WindowState ^= WindowState.Maximized); // Changes between normal and maximized
            CloseCommand = new Base.RelayCommand(() => _window.Close());

            SystemMenuCommand = new Base.RelayCommand(() => SystemCommands.ShowSystemMenu(_window, GetMousePosition()));

            ShowWindow = new Base.RelayCommand(() => MessageBox.Show("Hey!"));

            ShowHomePageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Home(this)));
            ShowAssetsPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Assets(this)));
            ShowTagPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Tags(this)));
            ShowLogPageCommand = new Base.RelayCommand(() => ChangeMainContent(new Views.Logs(this)));
            ReloadSplashCommand = new Base.RelayCommand(() => (splashScreen.DataContext as ViewModels.SplashViewModel).Reload());
            AddNotificationTestCommand = new Base.RelayCommand(() => AddNotification(message: "Test", background: Brushes.Green, foreground: Brushes.White));

            // Fixes window sizing issues at maximized
            var resizer = new Resources.Window.WindowResizer(_window);
        }

        private List<Department> GetDepartments()
        {
            if (DisplayCurrentDepartment)
                return new DepartmentRepository().GetAll();
            else
                return new List<Department>();
        }

        #endregion
        
        #region Private Members

        // The window this view model controls
        private Window _window;

        // Margin around the window to allow a drop shadow
        private int _outerMarginSize;

        private List<Page> pages = new List<Page>();

        private List<Page> excludedPages = new List<Page> {
            new Views.AssetManager(null),
            new Views.TagManager(null)
        };

        #endregion

        #region Public Propterties

        // The current page of the application
        public DataModels.ApplicationPage CurrentPage { get; set; }

        // The smallest size the window can have 
        public double WindowMinWidth { get; set; }
        public double WindowMinHeight { get; set; }

        // Size of padding of the inner content of the main window
        public int InnerContentPaddingSize { get; set; }

        // Padding of the inner content of the main window
        public Thickness InnerContentPadding { get { return new Thickness(0); } }

        // Size of the resize border around the window
        public int ResizeBorder { get; set; }

        // The size of the resize border around the window, taking the outer margin into account
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        // Margin around the window to allow a drop shadow. Checks if the window is maximised
        public int OuterMarginSize
        {
            get
            {
                // If the window is maximised, remove the margin around the window, we don't need the drop shadow
                return _window.WindowState == WindowState.Maximized ? 0 : _outerMarginSize;
            }

            set
            {
                _outerMarginSize = value;
            }
        }

        // Thickness of the margin around the window to allow a drop shadow.
        public Thickness OuterMarginThicknessSize { get { return new Thickness(OuterMarginSize); } }

        // The height of the title bar / caption of the window
        public int TitleHeight { get; set; }

        public int NavigationHeight { get; set; }

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        public String CurrentUser { get; set; }

        public Department CurrentDepartment { get; set; }

        public Frame FrameMainContent { get; set; } = new Frame();

        public Frame FrameSplash { get; set; } = new Frame();

        public Visibility SplashVisibility { get; set; } = Visibility.Visible;

        public List<Department> Departments { get => GetDepartments(); }

        public bool DisplayCurrentDepartment { get; set; } = false;

        public Session CurrentSession { get; private set; }

        public ObservableCollection<Notification> ActiveNotifications { get; set; } = new ObservableCollection<Notification>();

        #endregion

        #region Public Methods


        //public TopNavigationPart2 topNavigationPage;
        //public LeftNavigation leftNavigationPage;

        /// <summary>
        /// Changes how many rows or columns a specific frame spans over. 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="dir"></param>
        public void ChangeFrameExpasion(Frame frame, string dir)
        {
            if (dir == FrameExpansionEventArgs.Right)
                Grid.SetColumnSpan(frame, 10);
            else if (dir == FrameExpansionEventArgs.Left)
                Grid.SetColumnSpan(frame, 1);
            else if (dir == FrameExpansionEventArgs.Down)
                Grid.SetRowSpan(frame, 10);
            else if (dir == FrameExpansionEventArgs.Up)
                Grid.SetRowSpan(frame, 1);
        }

        /// <summary>
        /// Changes the content for the main content frame to the new page. If the page exists in the
        /// list of loaded pages, that one would be used. One can also specify a different frame of 
        /// which content will be modified to contain the new page.
        /// </summary>
        public void ChangeMainContent(Page newPage) => ChangeMainContent(newPage, FrameMainContent);

        public void ChangeMainContent(Page newPage, Frame frame)
        {
            Page setPage = null;
            // Search the loaded page list, for the given page to check if it has allready been loaded.
            foreach (Page page in pages)
            {
                if (page.GetType() == newPage.GetType())
                {
                    Console.WriteLine("Found new page in pages.");
                    setPage = page;
                }
            }

            // If the new page wasn't found in the list, the given newPage object is used and added to the list of pages.
            if (setPage == null)
            {
                Console.WriteLine("Unable to find new page in pages. Creating new page.");
                setPage = newPage;
                if (!ExcludedFromSaving(setPage))
                    pages.Add(setPage);
            }

            // Setting the content of the given frame, to the newPage object to display the requested page.
            frame.Content = setPage;
        }

        public async void AddNotification(string message, SolidColorBrush foreground, SolidColorBrush background)
        {
            Notification n = new Notification(message, foreground, background);
            ActiveNotifications.Add(n);

            await Task.Delay(2000);

            RemoveNotification(n);
        }

        private void RemoveNotification(Notification n)
        {
            ActiveNotifications.Remove(n);
        }

        public void ShowNotification(object a, object b) { }

        /// <summary>
        /// Used when the application has connected to the database and other external services,
        /// to remove the splash page and show the navigation menu's and homepage.
        /// </summary>
        public void SystemLoaded(Session session)
        {
            // Remove reload splash screen menuitem
            ReloadSplashCommand = null;

            AddNotification("test", Brushes.White, Brushes.Green);

            // Remove splash page
            SplashVisibility = Visibility.Hidden;

            // Show department and username
            DisplayCurrentDepartment = true;
            CurrentSession = session;
            CurrentUser = CurrentSession.Username;

            // Set current department
            if (Departments.Count > 0)
                CurrentDepartment = Departments[0];

            ChangeMainContent(new Views.Home(this));
        }

        #endregion

        #region Private Methods

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

        private void SelectDepartment()
        {

        }

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }

        public ICommand MaximizeCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand SystemMenuCommand { get; set; }

        public ICommand ShowWindow { get; set; }

        public ICommand ShowHomePageCommand { get; set; }

        public ICommand ShowAssetsPageCommand { get; set; }

        public ICommand ShowTagPageCommand { get; set; }

        public ICommand SelectDepartmentCommand { get; set; }

        public ICommand ShowLogPageCommand { get; set; }

        public ICommand ReloadSplashCommand { get; set; }

        public ICommand AddNotificationTestCommand { get; set; }

        #endregion

        #region Private Helpers

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

        #endregion
    }
}
