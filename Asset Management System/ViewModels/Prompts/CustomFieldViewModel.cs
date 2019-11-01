using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.ViewModels.Prompts
{
    public class CustomFieldViewModel : PromptViewModel
    {
        public override event PromptEventHandler PromptElapsed;


        public CustomFieldViewModel(string message, PromptEventHandler handler)
            : base(message, handler)
        {

        }


        protected override void Accept()
        {
            throw new NotImplementedException();
        }

        protected override void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
