using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Prompts
{
    class TextInputViewModel : PromptViewModel
    {
        public override event PromptEventHandler PromptElapsed;

        public string InputText { get; set; }

        public TextInputViewModel(string message, PromptEventHandler handler)
            : this(message, null, handler) { }

        public TextInputViewModel(string message, string startingText, PromptEventHandler handler)
            : base(message, handler) 
        {
            InputText = startingText;
        }

        protected override void Accept()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(true, InputText));
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
}
