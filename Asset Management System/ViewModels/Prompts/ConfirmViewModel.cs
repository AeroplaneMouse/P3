using Asset_Management_System.Events;
using System.Windows;

namespace Asset_Management_System.ViewModels.Prompts
{
    public class ConfirmViewModel : PromptViewModel
    {
        public override event PromptEventHandler PromptElapsed;
        //public IInputElement StartSelected { get; set; } = ;

        public ConfirmViewModel(string message, PromptEventHandler handler)
            : base(message, handler)
        {
        }

        protected override void Accept()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(true));
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
}