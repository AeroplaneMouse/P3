using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AMS.ViewModels.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // The event that is fired when any child property changes its value
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}