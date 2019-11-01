using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void PromtEventHandler(object sender, PromtEventArgs e);

    public class PromtEventArgs
    {
        public bool Result;
        public string ResultMessage;


        public PromtEventArgs(bool result)
            : this(result, null) { }

        public PromtEventArgs(bool result, string resultMessage)
        {
            Result = result;
            ResultMessage = resultMessage;
        }
    }
}
