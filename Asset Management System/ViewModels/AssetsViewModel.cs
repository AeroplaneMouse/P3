using Asset_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels
{
    public class AssetsViewModel
    {
        #region Constructors

        public AssetsViewModel()
        {
            //AddNewCommand = new ViewModels.Base.RelayCommand(() => Main.ChangeSourceRequest(new NewAsset(Main)));
            //SearchCommand = new ViewModels.Base.RelayCommand(() => Search());
            //EditCommand = new ViewModels.Base.RelayCommand(() => Edit());
            //RemoveCommand = new ViewModels.Base.RelayCommand(() => Remove());
        }

        #endregion

        #region Private Properties

        #endregion

        #region Public Properties

        public TextBox TbSearch { get; set; }

        public List<Selector> SelectedItems { get; set; } = new List<Selector>();

        public string SearchText { get; set; } = "";

        private ObservableCollection<Asset> _list;

        public ObservableCollection<Asset> SearchList
        {
            get
            {
                if (_list == null)
                    _list = new ObservableCollection<Asset>();
                return _list;
            }
            set
            {
                _list.Clear();
                foreach (Asset asset in value)
                    _list.Add(asset);
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        #endregion
    }
}
