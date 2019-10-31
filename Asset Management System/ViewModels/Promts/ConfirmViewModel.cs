using Asset_Management_System.Events;

namespace Asset_Management_System.ViewModels.Promts
{
    public class ConfirmViewModel : PromtViewModel
    {
        public override event PromtEventHandler PromtElapsed;
        
        public ConfirmViewModel(string message)
            : base(message) { }

        protected override void Accept()
        {
            PromtElapsed?.Invoke(this, new PromtEventArgs(true));
        }

        protected override void Cancel()
        {
            PromtElapsed?.Invoke(this, new PromtEventArgs(false));
        }
    }
}
