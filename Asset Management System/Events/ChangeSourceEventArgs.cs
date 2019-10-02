using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Asset_Management_System.Events
{
    public delegate void ChangeSourceEventHandler(object sender, ChangeSourceEventArgs e);

    public class ChangeSourceEventArgs
    {
        public ChangeSourceEventArgs(Page newSource)
        {
            NewSource = newSource;
        }

        public Page NewSource { get; }

    }
}
