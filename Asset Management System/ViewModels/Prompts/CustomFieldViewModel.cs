using Asset_Management_System.Events;
using Asset_Management_System.Models;
using System;
using System.Collections.Generic;

namespace Asset_Management_System.ViewModels.Prompts
{
    public class CustomFieldViewModel : PromptViewModel
    {
        private Field _newField;
        private bool _isCustom;

        public override event PromptEventHandler PromptElapsed;

        public string Name { get; set; } = String.Empty;
        public string DefaultValue { get; set; } = String.Empty;
        public bool IsRequired { get; set; } = false;
        public Field.FieldType SelectedFieldType { get; set; }
        public List<Field.FieldType> FieldTypes { get; set; } = (List<Field.FieldType>)Field.GetTypes();

        public CustomFieldViewModel(string message, PromptEventHandler handler, bool isCustom = false)
            : base(message, handler)
        {
            _isCustom = isCustom;
        }


        protected override void Accept()
        {
            _newField = new Field(Name, "", SelectedFieldType, DefaultValue, IsRequired, _isCustom);
            PromptElapsed?.Invoke(this, new PromptEventArgs(true, _newField));
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
}