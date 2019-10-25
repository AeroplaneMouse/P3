using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Asset_Management_System.Helpers;
using Asset_Management_System.Resources;
using Asset_Management_System.Views;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.ViewModels
{
    public class TagsViewModel : ChangeableListPageViewModel<TagRepository, Tag>
    {
        #region Constructors

        public TagsViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
        }

        #endregion

        #region Public Properties

        public int ViewType => 2;

        #endregion

        
        #region Methods

        protected override void View()
        {
            Console.WriteLine("Tag view");

            Tag selected = GetSelectedItem();
            
            Main.ChangeMainContent(new ObjectViewer(Main, selected));
        }

        #endregion
    }
}