using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Linq;

namespace Asset_Management_System.Database
{
    public class Statement
    {
        private List<Statement> _whereStatements;
        public string Column { get; }
        public string Operators { get; }
        public string Value { get; }

        public Statement() : this(null, null, null) { }
        
        public Statement(string column, string value, string operators="=")
        {
            Column = column;
            Operators = operators;
            Value = value;
            
            _whereStatements = new List<Statement>();
        }

        public void AddOrStatement(string column, string value, string operators="=")
        {
            _whereStatements.Add(new Statement(column, value, operators));
        }

        public string Render(bool withValues=false)
        {
            StringBuilder _queryPath = new StringBuilder();
            
            if (_whereStatements.Count > 0)
            {
                _queryPath.Append("("+string.Join(" OR ", from item in _whereStatements select item.Render())+")");
            }
            else
            {
                _queryPath.AppendFormat("{0} {1} {2}", Column, Operators, MySqlHelper.EscapeString(Value));
            }

            return _queryPath.ToString();
        }
    }
}