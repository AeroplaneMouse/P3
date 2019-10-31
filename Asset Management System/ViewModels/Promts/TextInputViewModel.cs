using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Asset_Management_System.ViewModels.Promts
{
    class TextInputViewModel : PromtViewModel
    {
        public override event PromtEventHandler PromtElapsed;

        public string InputText { get; set; }

        public TextInputViewModel(string message, PromtEventHandler handler)
            : this(message, null, handler) { }

        public TextInputViewModel(string message, string startingText, PromtEventHandler handler)
            : base(message, handler) 
        {
            InputText = startingText;
        }

        protected override void Accept()
        {
            PromtElapsed?.Invoke(this, new PromtEventArgs(true, InputText));
        }

        protected override void Cancel()
        {
            PromtElapsed?.Invoke(this, new PromtEventArgs(false));
        }
    }
}
