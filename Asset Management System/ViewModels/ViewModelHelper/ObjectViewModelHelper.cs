using System.Collections.ObjectModel;
using System.Windows.Input;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels.Commands;
using Asset_Management_System.ViewModels.Controllers;

namespace Asset_Management_System.ViewModels.ViewModelHelper
{
    public class ObjectViewModelHelper: Base.BaseViewModel
    {
        protected ObjectManagerController _objectManagerController = new ObjectManagerController();

        public ObservableCollection<ShownField> FieldsList = new ObservableCollection<ShownField>();
        // 
        public ObservableCollection<ShownField> HiddenFields = new ObservableCollection<ShownField>();
        // => 

        public ObservableCollection<ITagable> CurrentlyAddedTags;

        protected ICommand AddFieldCommand;
        // new AddFieldCommand(_main, this, true);
        protected ICommand RemoveFieldCommand;
        // new RemoveFieldCommand(this);
    }
}