using AMS.Models;

namespace AMS.Events
{
    public class FieldInputPromptEventArgs : PromptEventArgs
    {
        public Field Field;

        public FieldInputPromptEventArgs(bool result, Field field)
            : base(result)
        {
            Field = field;
        }
    }
}
