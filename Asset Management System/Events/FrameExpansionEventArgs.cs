using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Asset_Management_System.Events
{
    public delegate void FrameExpansionEventHandler(object sender, FrameExpansionEventArgs e);

    public class FrameExpansionEventArgs : EventArgs
    {
        public static int Collapse = 0;
        public static int Extend = 1;
        public static string Right = "right";
        public static string Left = "left";
        public static string Down = "down";
        public static string Up = "up";

        public int NewFrameMode = Collapse;
        public int OldFrameMode;
        public string Direction;
        public Frame Frame;

        public FrameExpansionEventArgs(int mode, string dir)
            : this(mode, dir, null) { }

        public FrameExpansionEventArgs(int mode, string dir, Frame frame)
        {
            OldFrameMode = NewFrameMode;
            NewFrameMode = mode;
            Direction = dir;
            Frame = frame;
        }

    }
}
