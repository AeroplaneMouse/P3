using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Database.Repositories;
using System.Windows;

namespace Asset_Management_System.ViewModels
{
    public abstract class ChangeableListPageViewModel<RepositoryType, T> : ListPageViewModel<RepositoryType, T>
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : class, new()
    {
        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        #endregion

        #region Constructor

        public ChangeableListPageViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
            // AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            AddNewCommand = new Base.RelayCommand(AddNew);
            EditCommand = new Base.RelayCommand(Edit);
            RemoveCommand = new Base.RelayCommand(Remove);
        }

        #endregion

        #region Methods

        protected void AddNew()
        {
            switch (PageType)
            {
                case ListPageType.Asset:
                    Main.ChangeMainContent(new Views.AssetManager(Main));
                    break;

                case ListPageType.Tag:
                    Main.ChangeMainContent(new Views.TagManager(Main));
                    break;

                default:
                    Console.WriteLine("Error when adding new");
                    break;
            }
        }

        protected void Edit()
        {
            T selected = GetSelectedItem();
            Console.WriteLine("Check: " + SelectedItemIndex);

            if (selected != null)
            {
                if (selected is Asset)
                {
                    Main.ChangeMainContent(new Views.AssetManager(Main, selected as Asset));
                }

                else if (selected is Tag)
                {
                    Main.ChangeMainContent(new Views.TagManager(Main, selected as Tag));
                }

                else
                {
                    Console.WriteLine("Fejl ved edit");
                }
            }
        }

        protected void Remove()
        {
            T selected = GetSelectedItem();

            if (selected != null)
            {
                if (selected is Asset)
                {
                    Log<Asset>.CreateLog(selected as ILoggable<Asset>, true);
                    (Rep as AssetRepository).Delete(selected as Asset);
                }

                else if (selected is Tag)
                {
                    Log<Tag>.CreateLog(selected as ILoggable<Tag>, true);
                    (Rep as TagRepository).Delete(selected as Tag);
                }

                else
                {
                    Console.WriteLine("Fejl ved Remove");
                }
            }

            Search();
        }

        #endregion


        #region Helpers

        public Visibility IsRemoveVisible { get; set; } = Visibility.Hidden;

        public string Title { get; set; }

        #endregion
    }
}