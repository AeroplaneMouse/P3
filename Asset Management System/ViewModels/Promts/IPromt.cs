using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Promts
{
    internal interface IPromt
    {
        public event Events.PromtEventHandler PromtElapsed;

        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }




    }
}