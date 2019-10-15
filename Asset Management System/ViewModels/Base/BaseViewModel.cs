using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace Asset_Management_System.ViewModels.Base
{
    /// <summary>
    /// Base view model that fires Property Changed events as needed
    /// </summary>
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

        #region Command Helpers

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true, the function is already running, then the action is not to run.
        /// If the flag is false, the function is not running already, the the action it to run.
        /// Once the action is finished, if it ran, then the flag is reset to false.
        /// </summary>
        /// <param name="updatingFlag">The boolean property flag indicating if the function is already running </param>
        /// <param name="action">The action to run if the funtion is not already running </param>
        /// <returns></returns>
        //protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        //{
        //    // Check if the flag property is set to true, meaning the funtion is already running
        //    if (updatingFlag.GetPropertyValue())
        //    {
        //        return;
        //    }

        //    // Set the property flag to true, to indicate that the function is running
        //    updatingFlag.SetPropertyValue(true);

        //    try
        //    {
        //        // Run the passed in action
        //        await action();
        //    }

        //    finally
        //    {
        //        // Set the property of the flag back to false, now that it is finished
        //        updatingFlag.SetPropertyValue(false);
        //    }


        //}

        #endregion
    }
}
