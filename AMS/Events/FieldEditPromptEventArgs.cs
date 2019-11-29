using AMS.Models;

namespace AMS.Events
{
    public class FieldEditPromptEventArgs : PromptEventArgs
    {
        public Field OldField;
        public Field NewField;

        public FieldEditPromptEventArgs(bool result, Field oldField, Field newField)
            : base(result)
        {
            OldField = oldField;
            NewField = newField;
        }
    }
}
