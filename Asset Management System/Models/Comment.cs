﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Asset_Management_System.Models
{
    class Comment
    {
        private string _comment = String.Empty;
        public Comment(string comment)
        {
            _comment = comment;
        }

        public override string ToString() => _comment;
    }
}
