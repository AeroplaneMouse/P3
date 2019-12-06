using AMS.Events;
using System.Windows.Input;

namespace AMS.ViewModels.Prompts
{
    internal interface IPrompt
    {
        event PromptEventHandler PromptElapsed;

        ICommand AcceptCommand { get; set; }
        ICommand CancelCommand { get; set; }
    }
}