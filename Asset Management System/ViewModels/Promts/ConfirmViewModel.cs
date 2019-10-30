using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Promts
{
    class ConfirmViewModel : Base.BaseViewModel
    {
        private bool result;
        private Action confirmed;

        public ICommand AcceptedCommand { get; set; }
        public ICommand CancelledCommand { get; set; }

        public ConfirmViewModel(Action confirmed, out bool promtResult)
        {
            this.confirmed = confirmed;
            promtResult = result;
            AcceptedCommand = new Base.RelayCommand(Accept);
            CancelledCommand = new Base.RelayCommand(Cancel);
        }

        private void Accept()
        {
            result = true;
            confirmed();
        }

        private void Cancel()
        {
            result = false;
            confirmed();
        }
    }
}
