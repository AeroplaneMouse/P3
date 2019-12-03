using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Events
{
    public class ExpandedPromptEventArgs : PromptEventArgs
    {
        public int ButtonNumber;
        public ExpandedPromptEventArgs(bool result, int pressedButtonNumber) 
            : base(result)
        {
            ButtonNumber = pressedButtonNumber;
        }
    }
}
