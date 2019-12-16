using AMS.Models;

namespace AMS.Events
{
    public class FunctionEditPromptEventArgs : PromptEventArgs
    {
        public Function OldFunction;
        public Function NewFunction;

        public FunctionEditPromptEventArgs(bool result, Function oldFunction, Function newFunction)
            : base(result)
        {
            OldFunction = oldFunction;
            NewFunction = newFunction;
        }
    }
}