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

        private string _groupString { get; set; }

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
                if (_tagList == null)
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

        public ICommand BackspaceKeyCommand { get; set; }

        public ICommand ShowBoxCommand { get; set; }

        #endregion


        #region Constructor

        public HomeViewModelTest()
        {
            Rep = new Database.Repositories.TagRepository();

            EnterKeyCommand = new Base.RelayCommand(() => 
            {
                // DO SOMETHING
                _tabIndex = 0;
            });

            TabKeyCommand = new Base.RelayCommand(() =>
            {
                _tagString = _tagList.Select(p => p.Label).ElementAtOrDefault(_tabIndex++);
                CheckString(_tagString);
            });

            SpaceKeyCommand = new Base.RelayCommand(() =>
            {
                if (_parentID == 0)
                {
                    ulong tempID = _tagList.Select(p => p.ID).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                    List<Models.Tag> tempList = Rep.GetChildTags(tempID);

                    if (tempList.Count() != 0)
                    {
                        _groupString = _tagList.Select(p => p.Label).ElementAtOrDefault(_tabIndex == 0 ? 0 : _tabIndex - 1);
                        _tagString = String.Empty;
                        _parentID = tempID; 
                        _tagList = tempList;

                        _tabIndex = 0;
                    }
                }
            });

            EscapeKeyCommand = new Base.RelayCommand(() =>
            {
                _tagString = String.Empty;
                _parentID = 0;
                _tagList = Rep.GetChildTags(0);
                _tabIndex = 0;
            });

            BackspaceKeyCommand = new Base.RelayCommand(() =>
            {
                if (_tagString == String.Empty)
                {
                    _parentID = 0;
                    _tagList = Rep.GetChildTags(_parentID);

                    _tagString = _groupString;
                    CheckString(_tagString);
                    return;
                }

                _tagString = _tagString.Substring(0, _tagString.Length - 1);
                CheckString(_tagString);

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
