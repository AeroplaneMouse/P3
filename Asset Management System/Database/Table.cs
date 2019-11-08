using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Google.Protobuf.WellKnownTypes;
using System.Linq;

namespace Asset_Management_System.Database
{
    public class Table
    {
        public string Name;
        public string JoinType;
        public List<Statement> Statements;

        public Table(string name, string joinType="")
        {
            Name = name;
            JoinType = joinType;
            Statements = new List<Statement>();
        }

        public void AddConnection(string column1, string column2, string operators="=")
        {
            Statements.Add(new Statement(column1, column2, operators));
        }

        public string Render()
        {
            if (JoinType == "")
            {
                return Name;
            }
            
            StringBuilder query_path = new StringBuilder();
            query_path.AppendFormat("{0} {1} ON ", JoinType, Name);
            query_path.Append(string.Join(" AND ", from item in Statements select $"{item.Column} {item.Operators} {item.Value}"));
            return query_path.ToString();
        }
    }
}