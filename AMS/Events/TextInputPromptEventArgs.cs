namespace AMS.Events
{
    public class TextInputPromptEventArgs : PromptEventArgs
    {
        public string Text;

        public TextInputPromptEventArgs(bool result, string resultMessage)
            : base(result)
        {
            Text = resultMessage;
        }
    }
}
