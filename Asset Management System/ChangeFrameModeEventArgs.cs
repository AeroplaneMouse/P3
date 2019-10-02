﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System
{
    public class ChangeFrameModeEventArgs : EventArgs
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

        public ChangeFrameModeEventArgs(int mode, string dir)
        {
            OldFrameMode = NewFrameMode;
            NewFrameMode = mode;
            Direction = dir;
        }

    }
}
