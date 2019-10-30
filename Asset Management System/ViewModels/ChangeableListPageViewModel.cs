using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;

namespace Asset_Management_System.ViewModels
{
    public abstract class ChangeableListPageViewModel<RepositoryType, T> : ListPageViewModel<RepositoryType, T>
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : class, new()
    {
        #region Commands

        public static ICommand AddNewCommand { get; set; }
        public static ICommand EditCommand { get; set; }
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
            Console.WriteLine("Add new");
            switch(PageType)
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
            Console.WriteLine("Edit");
            T selected = GetSelectedItem();

            if (selected != null)
            {
                switch (PageType)
                {
                    case ListPageType.Asset:
                        Main.ChangeMainContent(new Views.AssetManager(Main, selected as Asset));
                        break;

                    case ListPageType.Tag:
                        Main.ChangeMainContent(new Views.TagManager(Main, selected as Tag));
                        break;

                    default:
                        Console.WriteLine("Fejl ved Edit");
                        break;
                }
            }
        }

        protected void Remove()
        {
            Console.WriteLine("Remove");
            T selected = GetSelectedItem();

            if (selected != null)
            {
                switch (PageType)
                {
                    case ListPageType.Asset:
                        Log<Asset>.CreateLog(selected as ILoggable<Asset>, true);
                        (Rep as Database.Repositories.AssetRepository).Delete(selected as Asset);
                        break;

                    case ListPageType.Tag:
                        Log<Tag>.CreateLog(selected as ILoggable<Tag>, true);
                        (Rep as Database.Repositories.TagRepository).Delete(selected as Tag);
                        break;

                    default:
                        Console.WriteLine("Fejl ved Remove");
                        break;
                }
            }

            Search();
        }

        #endregion

        #region Helpers

        #endregion
    }
}
