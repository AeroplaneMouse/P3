using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class QueryGenerator : IDisposable
    {
        public List<string> Columns;
        public List<string> Values;
        public List<Table> Tables;
        public List<Statement> WhereStatements;
        private List<OrderBy> _orderBys;
        private string _groupBy;
        private StringBuilder _query;

        public QueryGenerator()
        {
            Columns = new List<string>();
            Values = new List<string>();
            Tables = new List<Table>();
            WhereStatements = new List<Statement>();
            _query = new StringBuilder();
        }

        public void AddTable(string name)
        {
            Tables.Add(new Table(name));
        }

        public void OrderBy(string column, string order="asc")
        {
            
        }

        public void Where(string column, string value, string operators="=")
        {
            WhereStatements.Add(new Statement(column, value, operators));
        }
        
        public void Where(Statement statement)
        {
            WhereStatements.Add(statement);
        }

        private string GetColumns()
        {
            if (Columns != null && Columns.Count > 0)
            {
                return string.Join(", ", Columns);
            }

            return "*";
        }

        public string PrepareSelect()
        {
            _query.Append("SELECT " + GetColumns());
            _query.Append(" FROM " + string.Join(" ", from item in Tables select item.Render()));

            if (WhereStatements.Count > 0)
            {
                _query.Append(" WHERE " + string.Join(" AND ", from item in WhereStatements select item.Render()));
            }

            return _query.ToString();
        }

        public string PrepareInsert()
        {
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                _query.Append("INSERT INTO "+Tables[0].Name);
                _query.Append(" FROM (" + GetColumns() + ") ");
                string columnValuePairs;
                int initialValue = Columns.Count - 1;

                columnValuePairs = (new Statement(Columns[initialValue], Values[initialValue])).Render();

                for (int i = initialValue - 1; i >= 0; i--)
                {
                    columnValuePairs += $", {(new Statement(Columns[i], Values[i])).Render()}";
                }

                _query.Append($" VALUES ( {columnValuePairs} )");
                return _query.ToString();
            }

            return "";
        }

        public string PrepareUpdate()
        {
            //Checks if there is added any tables and if the number of columns and values are the same, to ensure success
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                //Create the query string
                _query.Append("UPDATE " + Tables[0].Name);

                string columnValuePairs;
                int initialValue = Columns.Count - 1;

                columnValuePairs = (new Statement(Columns[initialValue], Values[initialValue])).Render();

                for (int i = initialValue - 1; i >= 0; i--)
                {
                    columnValuePairs += $", {(new Statement(Columns[i], Values[i])).Render()}";
                }

                _query.Append(" SET " + columnValuePairs);
                
                if (WhereStatements.Count > 0)
                {
                    _query.Append(" WHERE " + string.Join(" AND ", from item in WhereStatements select item.Render()));
                }

                return _query.ToString();
            }

            return "";
        }

        public string PrepareDelete()
        {
            return "";
        }
        
        public void Dispose()
        {
        }
    }
}