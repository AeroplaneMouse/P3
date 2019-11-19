namespace AMS.Events
{
    public class PromptEventArgs
    {
        public bool Result;

        public PromptEventArgs(bool result)
        {
            Result = result;
        }
    }
}
