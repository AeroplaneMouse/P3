using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using AMS.Events;

namespace AMS.ViewModels.Prompts
{
    public class ExpandedConfirmViewModel : PromptViewModel
    {
        public override event PromptEventHandler PromptElapsed;

        public ObservableCollection<string> Buttons { get; set; }

        public ICommand ButtonPressedCommand { get; set; }

        public ExpandedConfirmViewModel(string message, List<string> buttons, PromptEventHandler handler)
            : base (message, handler)
        {
            Buttons = new ObservableCollection<string>(buttons);

            OnPropertyChanged(nameof(Buttons));
            ButtonPressedCommand = new Base.RelayCommand<object>(ButtonPressed);
        }

        private void ButtonPressed(object obj)
        {
            if (obj is string buttonText)
            {
                int buttonNumber = 0;

                // Find the number of the pressed button
                foreach(string text in Buttons)
                {
                    if (text == buttonText)
                        break;
                    buttonNumber++;
                }

                PromptElapsed?.Invoke(this, new ExpandedPromptEventArgs(true, buttonNumber));
            }
            else
            {
                // Error. Unknown button
            }
        }

        protected override void Accept()
        {
            throw new NotImplementedException();
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
}
