using System;
using AMS.Models;

namespace AMS.Events
{
    public class FunctionInputPromptEventArgs : PromptEventArgs
    {
        public Function Function;
        public FunctionInputPromptEventArgs(bool result, Function function) : base(result)
        {
            Function = function;
        }
    }
}