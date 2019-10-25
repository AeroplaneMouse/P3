using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.ViewModels
{
    class PopupViewModel : Base.BaseViewModel
    {
        private MainViewModel _main;
        public double WindowHeight { get; set; }


        public PopupViewModel(MainViewModel main)
        {
            _main = main;
            WindowHeight = main.GetWindowHeight();
            main.NotifyOnWindowResize(WindowSizeChanged);

        }

        public void WindowSizeChanged(object sender, EventArgs e)
        {
            Console.WriteLine("The window ");
        }



    }
}
