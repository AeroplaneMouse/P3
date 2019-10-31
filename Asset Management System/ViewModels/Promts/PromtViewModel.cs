using System.Windows.Input;
using Asset_Management_System.Events;

namespace Asset_Management_System.ViewModels.Promts
{
    public abstract class PromtViewModel : Base.BaseViewModel, IPromt
    {
        public abstract event PromtEventHandler PromtElapsed;
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string MessageText { get; set; }


        public PromtViewModel(string message)
        {
            MessageText = message;

            // Initializing commands
            AcceptCommand = new Base.RelayCommand(Accept);
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        protected abstract void Accept();

        protected abstract void Cancel();

    }
}
