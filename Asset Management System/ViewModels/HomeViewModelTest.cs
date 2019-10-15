using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace Asset_Management_System.ViewModels
{
    class HomeViewModelTest : Base.BaseViewModel
    {
        #region Private Members

        private string _tagString { get; set; }

        private ulong _parentID { get; set; } = 0;

        private List<Models.Tag> _tagList { get; set; }

        private int _tabIndex { get; set; } = 0;

        #endregion


        #region Public Properties

        public string ParentID 
        { 
            get
            {
                if (_parentID == 0)
                {
                    return "Tag groups:";
                }

                return Rep.GetById(_parentID).Label + ":";
            } 
        }

        public List<Models.Tag> TagList 
        { 
            get
            {
                if (_tagList == null || _tagList.Count() == 0)
                {
                    _tagList = Rep.GetChildTags(_parentID);
                }

                return _tagList.Take(10).ToList();
            }
        }

        public string TagString
        {
            get
            {
                return _tagString;
            }

            set
            {
                Tags = Rep.GetChildTags(_parentID);

                CheckString(value);

                _tabIndex = 0;

                _tagString = value;
            }
        }

        public List<Models.Tag> Tags { get; set; }

        public Database.Repositories.TagRepository Rep { get; set; }

        #endregion


        #region Commands

        public ICommand EnterKeyCommand { get; set; }

        public ICommand TabKeyCommand { get; set; }

        public ICommand SpaceKeyCommand { get; set; }

        public ICommand EscapeKeyCommand { get; set; }

        public ICommand ShowBoxCommand { get; set; }

        #endregion


        #region Constructor

        public HomeViewModelTest()
        {
            Rep = new Database.Repositories.TagRepository();

            EnterKeyCommand = new Base.RelayCommand(() => 
            {
                // SEARCH
                _tabIndex = 0;
            });

            TabKeyCommand = new Base.RelayCommand(() =>
            {
                _tagString = _tagList.Select(p => p.Label).ElementAtOrDefault(_tabIndex++);
            });

            SpaceKeyCommand = new Base.RelayCommand(() =>
            {
                if (_parentID == 0)
                {
                    _tagString = String.Empty;
                    _parentID = _tagList.Select(p => p.ID).First();//.ElementAtOrDefault(_tabIndex);
                    _tagList = Rep.GetChildTags(_parentID);
                    _tabIndex = 0;
                }
            });

            EscapeKeyCommand = new Base.RelayCommand(() =>
            {
                _tagString = String.Empty;
                _parentID = 0;
                _tagList = Rep.GetChildTags(0);
                _tabIndex = 0;
            });


            ShowBoxCommand = new Base.RelayCommand(() => MessageBox.Show("Hej!"));
        }

        #endregion

        public void CheckString(string value)
        {
            _tagList = Tags
                .Where(p => p.Label.ToLower().Contains(value.ToLower()))
                .OrderByDescending(p => p.Label.ToLower().StartsWith(value.ToLower()))
                .ToList();
        }
    }
}
