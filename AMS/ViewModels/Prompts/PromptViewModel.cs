using System.Windows.Input;
using AMS.Events;

namespace AMS.ViewModels.Prompts
{
    public abstract class PromptViewModel : Base.BaseViewModel, IPrompt
    {
        public abstract event PromptEventHandler PromptElapsed;
        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public string MessageText { get; set; }
        

        public PromptViewModel(string message, PromptEventHandler handler)
        {
            MessageText = message;
            PromptElapsed += handler;

            // Initializing commands
            AcceptCommand = new Base.RelayCommand(Accept);
            CancelCommand = new Base.RelayCommand(Cancel);
        }

        public override void UpdateOnFocus() { }

        protected abstract void Accept();

        protected abstract void Cancel();
    }
}