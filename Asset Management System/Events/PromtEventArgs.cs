using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Events
{
    public delegate void PromptEventHandler(object sender, PromptEventArgs e);

    public class PromptEventArgs
    {
        public bool Result;
        public string ResultMessage;


        public PromptEventArgs(bool result)
            : this(result, null) { }

        public PromptEventArgs(bool result, string resultMessage)
        {
            Result = result;
            ResultMessage = resultMessage;
        }
    }
}
