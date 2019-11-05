using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Prompts
{
    internal interface IPrompt
    {
        public event Events.PromptEventHandler PromptElapsed;

        public ICommand AcceptCommand { get; set; }
        public ICommand CancelCommand { get; set; }
    }
}