using AMS.Events;

namespace AMS.ViewModels.Prompts
{
    public class ConfirmViewModel : PromptViewModel
    {
        public override event PromptEventHandler PromptElapsed;
        
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