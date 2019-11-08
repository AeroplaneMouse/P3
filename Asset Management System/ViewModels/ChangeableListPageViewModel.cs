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
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public abstract class ChangeableListPageViewModel<T> : ListPageViewModel<T>
        where T : class, new()
    {
        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        #endregion

        private IDisplayableService<T> _service;

        #region Constructor

        public ChangeableListPageViewModel(MainViewModel main, IDisplayableService<T> service) : base(main, service)
        {
            // AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            AddNewCommand = new Base.RelayCommand(AddNew);
            EditCommand = new Base.RelayCommand(Edit);
            RemoveCommand = new Base.RelayCommand(Remove);

            _service = service;
        }

        #endregion

        #region Methods

        protected void AddNew()
        {
            Main.ChangeMainContent(_service.GetManagerPage(Main));
            /*
            switch (PageType)
            {
                case ListPageType.Asset:
                    Main.ChangeMainContent(new Views.AssetManager(Main)); // TODO: Get via the service?
                    break;

                case ListPageType.Tag:
                    Main.ChangeMainContent(new Views.TagManager(Main));
                    break;

                default:
                    Console.WriteLine("Error when adding new");
                    break;
            }
            */
        }

        protected void Edit()
        {
            T selected = GetSelectedItem();
            Console.WriteLine("Check: " + SelectedItemIndex);

            if (selected == null) return;
            Main.ChangeMainContent(_service.GetManagerPage(Main, selected));
            /*
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
            */
        }

        private Asset RemoveAsset;
        private Tag RemoveTag;

        protected void Remove()
        {
            var selected = SelectedItems[0];

            if (selected == null) return;
            _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete asset { _service.GetName(selected) }?", RemovePromptElapsed));
            /*
            switch (selected)
            {
                case Asset asset:
                    RemoveAsset = asset;
                    RemoveTag = null;
                    _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete { SelectedItems.Count } asset(s)?", RemovePromptElapsed));
                    break;
                case Tag tag:
                    RemoveAsset = null;
                    RemoveTag = tag;
                    _main.DisplayPrompt(new Views.Prompts.Confirm($"Are you sure you want to delete { SelectedItems.Count } tag(s) ?", RemovePromptElapsed));
                    break;
                default:
                    Console.WriteLine("Fejl ved Remove");
                    break;
            }
            /**/
        }

        private void RemovePromptElapsed(object sender, PromptEventArgs e)
        {
            if (!e.Result) return;
            Log<T>.CreateLog((ILoggable<T>)GetSelectedItem(), true); //TODO: Fix so typecast is unnecessary
            /*
            if (RemoveAsset != null)
            {
                Log<Asset>.CreateLog(RemoveAsset, true);
                Rep.Delete(RemoveAsset as T);
            }
            else if(RemoveTag != null)
            {
                foreach (var var in SelectedItems)
                {
                    if (RemoveAsset != null)
                    {
                        Log<Asset>.CreateLog(var as Asset, true);
                        (Rep as AssetRepository).Delete(var as Asset);
                    }
                    else if(RemoveTag != null)
                    {
                        Log<Tag>.CreateLog(var as Tag, true);
                        (Rep as TagRepository).Delete(var as Tag);
                    }
                    Search();
                }

            }
            */
            Search();
        }

        #endregion


        #region Helpers

        public Visibility IsRemoveVisible { get; set; } = Visibility.Hidden;

        public string Title { get; set; }

        #endregion
    }
}
