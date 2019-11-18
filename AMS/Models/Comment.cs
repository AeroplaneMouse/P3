using System;
using System.Collections.Generic;
using System.Text;

namespace AMS.Models
{
    public class Comment : Model
    {
        public object AssetID;
        public object Username { get; set; }
        public object Content { get; set; }
    }
}
