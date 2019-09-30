using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Asset_Management_System
{
    class Session
    {
        public Session()
        {

        }

        public SqlConnection Database { get; private set; }

    }
}
