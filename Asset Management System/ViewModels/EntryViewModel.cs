using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Views;

namespace Asset_Management_System.ViewModels
{
    public class EntryViewModel
    {
        #region Constructor

        /// <summary>
        /// Default contructor
        /// </summary>
        public EntryViewModel(Window window, Entry entry)
        {
            // Initialize commands
            ExitCommand = new Base.RelayCommand(() => Exit());

            _window = window;
            _entry = entry;
            this.LabelText = "Log entry: " + entry.Id.ToString();
            this.DescriptionText = entry.Description;
            this.UserText = "By: " + entry.Username;
            this.TimeText = "At: " + entry.DateToStringConverter;
            this.OptionsText = RemoveUnwantedChars(entry.Options);
        }

        #endregion

        #region Private Properties

        private Window _window;
        private Entry _entry;

        #endregion

        #region Public Properties

        public string LabelText { get; set; }

        public string DescriptionText { get; set; }

        public string UserText { get; set; }

        public string OptionsText { get; set; }

        public string TimeText { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Removes characters that make the options text less readable
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private string RemoveUnwantedChars(string options)
        {
            return options
                .Replace('{', '\n')
                .Replace('}', '\0')
                .Replace('\\', '\0')
                .Replace(',', '\n')
                .Replace('"', '\0');
        }

        /// <summary>
        /// Closes the entry window
        /// </summary>
        private void Exit()
        {
            _window.DialogResult = true;
        }

        #endregion

        #region Commands

        public ICommand ExitCommand { get; set; }

        #endregion
    }
}