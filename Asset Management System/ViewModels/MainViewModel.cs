using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Asset_Management_System.ViewModels
{
    public class MainViewModel : Base.BaseViewModel
    {
        #region Private Members

        // The window this view model controls
        private Window _window;

        // Margin around the window to allow a drop shadow
        private int _outerMarginSize;

        // Radius of the edges of the window
        //private int _windowRadius;

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

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        #endregion

        #region Commands

        public ICommand MinimizeCommand { get; set; }

        public ICommand MaximizeCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand SystemMenuCommand { get; set; }

        public ICommand ShowWindow { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel(Window window)
        {
            // Setting private fields
            _window = window;
            _outerMarginSize = 10;
            //_windowRadius = 5;

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


            // Initialize commands
            MinimizeCommand = new Base.RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new Base.RelayCommand(() => _window.WindowState ^= WindowState.Maximized); // Changes between normal and maximized
            CloseCommand = new Base.RelayCommand(() => _window.Close());

            SystemMenuCommand = new Base.RelayCommand(() => SystemCommands.ShowSystemMenu(_window, GetMousePosition()));

            ShowWindow = new Base.RelayCommand(() => MessageBox.Show("Hey!"));


            // Fixes window sizing issues at maximized
            var resizer = new Resources.Window.WindowResizer(_window);
        }

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
