using System;
using System.Collections;
using System.Windows.Media;

namespace AMS.Models
{
    public class Notification : IEqualityComparer
    {
        private static int _id = 0;

        // Static stuff
        public static readonly SolidColorBrush ERROR = Brushes.Red;
        public static readonly SolidColorBrush WARNING = Brushes.Orange;
        public static readonly SolidColorBrush INFO = Brushes.LightGray;
        public static readonly SolidColorBrush APPROVE = Brushes.Green;

        // Public properties
        public int ID { get; }
        public string Message { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush Foreground { get; set; }

        #region Constructor

        public Notification(string message) : this(message, RandomColor()) { }

        public Notification(string message, SolidColorBrush background)
        {
            ID = _id++;
            Message = message;
            Foreground = GetForegroundColor(background.Color);
            Background = background;
        }

        #endregion

        public new bool Equals(object x, object y)
        {
            if (x is Notification a && y is Notification b)
                return a.ID == b.ID;
            else
                return false;
        }

        public int GetHashCode(object obj) => (obj as Notification).ID;

        public static SolidColorBrush RandomColor()
        {
            int seed = new DateTime().Millisecond;
            Random r = new Random();

            SolidColorBrush[] backgrounds = new SolidColorBrush[]
            {
                Brushes.DarkGray,
                Brushes.Red,
                Brushes.Orange,
                Brushes.Green,
                Brushes.Transparent,
                Brushes.Blue,
                Brushes.DarkGreen,
                Brushes.DarkOrange,
                Brushes.LightBlue,
                Brushes.Cyan,
                Brushes.Firebrick
            };

            return backgrounds[r.Next() % backgrounds.Length];
        }

        // Borrowed from http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx
        /// <summary>
        /// Measures the apparent brightness of a color, and returns a number between 0 and 255
        /// </summary>
        /// <param name="c"></param>
        /// <returns>Brightness of the color</returns>
        private static int Brightness(Color c) => (int)Math.Sqrt(c.R * c.R * 0.241 + c.G * c.G * 0.691 + c.B * c.B * 0.068);

        public static SolidColorBrush GetForegroundColor(Color c) => Brightness(c) < 140 ? Brushes.White : Brushes.Black;

        public static SolidColorBrush GetForegroundColor(string colorString) => Brushes.Black;
    }
}
