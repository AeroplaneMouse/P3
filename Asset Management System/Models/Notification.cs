﻿using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Asset_Management_System.Models
{
    public class Notification : IEqualityComparer
    {
        private static int _id = 0;

        public readonly int ID;
        public string Message { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush Foreground { get; set; }

        #region Constructors
        public Notification(string message)
            : this(message, Brushes.White, Brushes.Yellow) { }

        public Notification(string message, SolidColorBrush background)
            : this(message, Brushes.White, background) { }

        public Notification(string message, SolidColorBrush foreground, SolidColorBrush background)
        {
            ID = _id++;
            Message = message;
            Foreground = foreground;
            Background = background;
        }

        public new bool Equals(object x, object y)
        {
            if (x is Notification a && y is Notification b)
                return a.ID == b.ID;
            else
                return false;
        }

        public int GetHashCode(object obj)
        {
            return (obj as Notification).ID;
        }
        #endregion

    }
}
