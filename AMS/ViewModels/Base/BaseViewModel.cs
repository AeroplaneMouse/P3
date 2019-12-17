using AMS.Interfaces;
using System.ComponentModel;

namespace AMS.ViewModels.Base
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IPageUpdateOnFocus
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

        /// <summary>
        /// When the page that this view model is attached to recieves focus again,
        /// update whatever is necessary to update
        /// </summary>
        public abstract void UpdateOnFocus();
    }
}