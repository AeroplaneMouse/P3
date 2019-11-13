using System;
using System.Windows;
using System.Windows.Input;
using Asset_Management_System.Events;
using Asset_Management_System.Models;
using Asset_Management_System.Logging;
using Asset_Management_System.Resources.DataModels;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Services.Interfaces;

namespace Asset_Management_System.ViewModels
{
    public abstract class ChangeableListPageViewModel<T> : ListPageViewModel<T>
        where T : class, new()
    {
        private Asset RemoveAsset;
        private Tag RemoveTag;

        public ICommand AddNewCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        private IDisplayableService<T> _service;
        
        public ChangeableListPageViewModel(MainViewModel main, IDisplayableService<T> service) : base(main, service)
        {
            // AddNewCommand = new ViewModels.Base.RelayCommand(() => _main.ChangeMainContent(new Views.TagManager(_main)));
            AddNewCommand = new Base.RelayCommand(AddNew);
            EditCommand = new Base.RelayCommand(Edit);
            RemoveCommand = new Base.RelayCommand(Remove);

            _service = service;
        }

        protected void AddNew()
        {
            _main.ChangeMainContent(_service.GetManagerPage(_main));
            /*
            switch (PageType)
            {
                case ListPageType.Asset:
                	Main.ChangeMainContent(new Views.AssetManager(Main)); // TODO: Get via the service?
                    break;

                case ListPageType.Tag:
                    _main.ChangeMainContent(new Views.TagManager(_main));
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

            if (selected == null) 
                return;
            _main.ChangeMainContent(_service.GetManagerPage(_main, selected));
            /*
            switch (selected)
            {
                case Asset asset:
                    _main.ChangeMainContent(new Views.AssetManager(_main, asset));
                    break;
                case Tag tag:
                    _main.ChangeMainContent(new Views.TagManager(_main, tag));
                    break;
                default:
                    Console.WriteLine("Fejl ved edit");
                    break;
            }
            */
        }
        

        protected void Remove()
        {
            var selected = SelectedItems[0];

            if (selected == null) 
                return;

            string message = $"Are you sure you want to delete { SelectedItems.Count } ";
            switch (selected)
            {
                case Asset asset:
                    RemoveAsset = asset;
                    RemoveTag = null;
                    message += "asset";
                    break;
                case Tag tag:
                    RemoveAsset = null;
                    RemoveTag = tag;
                    message += "tag";
                    
                    break;
                default:
                    _main.AddNotification(new Notification("Error! An unknown error occured...", Notification.ERROR));
                    Console.WriteLine("Fejl ved Remove");
                    return;
            }

            // Showing prompt
            message += SelectedItems.Count > 1 ? "s?" : "?";
            _main.DisplayPrompt(new Views.Prompts.Confirm(message, RemovePromptElapsed));
        }

        private void RemovePromptElapsed(object sender, PromptEventArgs e)
        {
            // Check if the pressed button was accept
            if (e.Result)
            {
                Log<T>.CreateLog((ILoggable<T>)GetSelectedItem(), true); //TODO: Fix so typecast is unnecessary
                if (RemoveAsset != null)
                    Rep.Delete(RemoveAsset as T);
                else if (RemoveTag != null)
                {
                    foreach (var var in SelectedItems)
                    {
                        Rep.Delete(var as T);
                        /*
                        if (RemoveAsset != null)
                            ((AssetRepository) Rep).Delete(var as Asset);

                        else if(RemoveTag != null)
                            ((TagRepository) Rep).Delete(var as Tag);
                        */
                        Search();
                    }
                }
                /**/
                Search();
            }
        }
    }
}