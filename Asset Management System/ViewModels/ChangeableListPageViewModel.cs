using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Asset_Management_System.Logging;
using Asset_Management_System.Models;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Database.Repositories;
using System.Windows;
using Asset_Management_System.Events;

namespace Asset_Management_System.ViewModels
{
    public abstract class ChangeableListPageViewModel<RepositoryType, T> : ListPageViewModel<RepositoryType, T>
        where RepositoryType : Database.Repositories.ISearchableRepository<T>, new()
        where T : class, new()
    {
        private Asset RemoveAsset;
        private Tag RemoveTag;

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public Visibility IsRemoveVisible { get; set; } = Visibility.Hidden;
        public string Title { get; set; }

        public ChangeableListPageViewModel(MainViewModel main, ListPageType pageType) : base(main, pageType)
        {
            // AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            AddNewCommand = new Base.RelayCommand(AddNew);
            EditCommand = new Base.RelayCommand(Edit);
            RemoveCommand = new Base.RelayCommand(Remove);
        }

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

            if (selected == null) return;
            switch (selected)
            {
                case Asset asset:
                    Main.ChangeMainContent(new Views.AssetManager(Main, asset));
                    break;
                case Tag tag:
                    Main.ChangeMainContent(new Views.TagManager(Main, tag));
                    break;
                default:
                    Console.WriteLine("Fejl ved edit");
                    break;
            }
        }
        

        protected void Remove()
        {
            T selected = GetSelectedItem();

            if (selected == null) 
                return;
            switch (selected)
            {
                case Asset asset:
                    RemoveAsset = asset;
                    RemoveTag = null;
                    _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete asset { asset.Name }?", RemovePromptElapsed));
                    break;
                case Tag tag:
                    RemoveAsset = null;
                    RemoveTag = tag;
                    _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete tag { tag.Name }?", RemovePromptElapsed));
                    break;
                default:
                    Console.WriteLine("Fejl ved Remove");
                    break;
            }
        }

        private void RemovePromptElapsed(object sender, PromptEventArgs e)
        {
            if (e.Result)
            {
                if (RemoveAsset != null)
                {
                    Log<Asset>.CreateLog(RemoveAsset, true);
                    (Rep as AssetRepository).Delete(RemoveAsset as Asset);
                }
                else if(RemoveTag != null)
                {
                    Log<Tag>.CreateLog(RemoveTag, true);
                    (Rep as TagRepository).Delete(RemoveTag as Tag);
                }
                Search();
            }
        }
    }
}