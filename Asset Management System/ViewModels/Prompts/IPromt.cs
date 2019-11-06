using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Prompts
{
    internal interface IPrompt
    {
        event Events.PromptEventHandler PromptElapsed;

        ICommand AcceptCommand { get; set; }
        ICommand CancelCommand { get; set; }
    }
}