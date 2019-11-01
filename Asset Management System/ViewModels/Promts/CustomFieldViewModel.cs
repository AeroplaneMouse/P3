using Asset_Management_System.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.ViewModels.Promts
{
    public class CustomFieldViewModel : PromtViewModel
    {
        public override event PromtEventHandler PromtElapsed;


        public CustomFieldViewModel(string message, PromtEventHandler handler)
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
