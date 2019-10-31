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

        public TextInputViewModel(string message)
            : base(message) { }

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
