using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using AMS.Events;
using AMS.Models;

namespace AMS.ViewModels.Prompts
{
    public class CustomFunctionViewModel : PromptViewModel
    {
        private Function _newFunction;
        private Function _oldFunction;

        public override event PromptEventHandler PromptElapsed;

        public bool IsFieldNameNotNull => !string.IsNullOrEmpty(Name);
        public string Name { get; set; } = String.Empty;
        public string DefaultValue { get; set; } = String.Empty;

        public string SelectedDate { get; set; } = "System.Windows.Controls.ComboBoxItem: Current Date";
        public bool IsRequired { get; set; } = false;
        public Function.FunctionType SelectedFunctionType { get; set; }

        public List<Function.FunctionType> FunctionTypes { get; set; } =
            (List<Function.FunctionType>) Function.GetTypes();

        public string PromptHeaderAndAcceptButtonText { get; set; }


        public CustomFunctionViewModel(string message, PromptEventHandler handler, Function inputFunction = null)
            : base(message, handler)
        {
            PromptHeaderAndAcceptButtonText = "Add function";
            _oldFunction = inputFunction;
            if (_oldFunction != null)
            {
                Name = inputFunction.Label;
                SelectedFunctionType = inputFunction.Type;

                if (SelectedFunctionType == Function.FunctionType.Expiration)
                    SelectedDate = inputFunction.Content;
                else
                    DefaultValue = inputFunction.Content;
            }
        }

        protected override void Accept()
        {
            if (SelectedFunctionType == 0)
                return;

            if (SelectedFunctionType == Function.FunctionType.Expiration)
            {
                DateTime.TryParse(DefaultValue, out var someDate);
                Console.WriteLine(someDate);
                if (someDate.ToString(CultureInfo.InvariantCulture) != "01/01/0001 00:00:00")
                {
                    
                    DefaultValue = (someDate - DateTime.Today).Days.ToString();
                }
                else
                {
                    Regex year = new Regex(@"\d[y]");
                    Regex months = new Regex(@"\d[m]");
                    Regex weeks = new Regex(@"\d[w]");
                    Regex days = new Regex(@"\d[d]");
                    int InputAsDays = 0;

                    string[] tempstring = DefaultValue.Split(" ");
                    foreach (var substring in tempstring)
                    {
                        if (year.IsMatch(substring))
                        {
                            InputAsDays += int.Parse(substring[0].ToString()) * 365;
                        }

                        if (months.IsMatch(substring))
                        {
                            InputAsDays += int.Parse(substring[0].ToString()) * 365;
                        }

                        if (weeks.IsMatch(substring))
                        {
                            InputAsDays += int.Parse(substring[0].ToString()) * 7;
                        }

                        if (days.IsMatch(substring))
                        {
                            InputAsDays += int.Parse(substring[0].ToString()) * 365;
                        }
                    }

                    DefaultValue = InputAsDays.ToString();
                }
            }

            _newFunction = new Function(Name, DefaultValue, SelectedFunctionType);

            if (_oldFunction == null)
                PromptElapsed?.Invoke(this, new FunctionInputPromptEventArgs(true, _newFunction));

            PromptElapsed?.Invoke(this, new FunctionEditPromptEventArgs(true, _oldFunction, _newFunction));
        }

        protected override void Cancel()
        {
            PromptElapsed?.Invoke(this, new PromptEventArgs(false));
        }
    }
}