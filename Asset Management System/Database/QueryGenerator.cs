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
        private Dictionary<string, string> _orderBys;
        private string _groupBy;
        private StringBuilder _query;

        public QueryGenerator()
        {
            Columns = new List<string>();
            Values = new List<string>();
            Tables = new List<Table>();
            WhereStatements = new List<Statement>();
            _orderBys = new Dictionary<string, string>();
            _query = new StringBuilder();
        }

        /// <summary>
        /// Adds a table to the query
        /// </summary>
        /// <param name="name"></param>
        public void AddTable(string name)
        {
            Tables.Add(new Table(name));
        }

        /// <summary>
        /// Sets the column to order the fetched data by and the order
        /// </summary>
        /// <param name="column"></param>
        /// <param name="order">'true' for ascending or 'false' for descending</param>
        public void OrderBy(string column, bool ascending = true)
        {
            _orderBys.Add(column, (ascending ? "ASC" : "DESC"));
        }

        /// <summary>
        /// Adds a 'where' condition to the query, based on column, value, and operator
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="operators"></param>
        public void Where(string column, string value, string operators="=")
        {
            WhereStatements.Add(new Statement(column, value, operators));
        }

        /// <summary>
        /// Adds a 'where' condition to the query, based on a statement
        /// </summary>
        /// <param name="statement"></param>
        public void Where(Statement statement)
        {
            WhereStatements.Add(statement);
        }

        /// <summary>
        /// Returns the columns contained in the query
        /// </summary>
        /// <returns>Comma separated columns of the query</returns>
        private string GetColumns()
        {
            if (Columns != null && Columns.Count > 0)
            {
                return string.Join(", ", Columns);
            }

            return "*";
        }

        /// <summary>
        /// Creates a query to select the columns of the query from the tables of the query
        /// </summary>
        /// <returns>Select query</returns>
        public string PrepareSelect()
        {
            _query.Append("SELECT " + GetColumns());
            _query.Append(" FROM " + string.Join(" ", from item in Tables select item.Render()));

            if (WhereStatements.Count > 0)
            {
                _query.Append(" WHERE " + string.Join(" AND ", from item in WhereStatements select item.Render()));
            }

            if (_orderBys.Count > 0)
            {
                _query.Append($" ORDER BY {_orderBys.First().Key} {_orderBys.First().Value}");

                foreach (KeyValuePair<string, string> keyValuePair in _orderBys.Skip(1))
                {
                    _query.Append($", {keyValuePair.Key} {keyValuePair.Value}");
                }
            }
            return _query.ToString();
        }

        /// <summary>
        /// Creates a query to insert a new row into the first table of the query, with the values of the query
        /// </summary>
        /// <returns>Insert query</returns>
        public string PrepareInsert()
        {
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                _query.Append("INSERT INTO " + Tables[0].Name);
                _query.Append(" ( " + GetColumns() + " )");
                string values;
                int length = Values.Count;

                values = (int.TryParse(Values[0], out int returnedInt) || bool.TryParse(Values[0], out bool returnedBool) ? Values[0] : $"'{Values[0]}'");

                for (int i = 1; i < length; i++)
                {
                    values += $", {(int.TryParse(Values[i], out returnedInt) || bool.TryParse(Values[i], out returnedBool) ? Values[i] : $"'{Values[i]}'")}";
                }

                _query.Append($" VALUES ( {values} )");
                return _query.ToString();
            }

            return "";
        }

        /// <summary>
        /// Creates a query to update the columns of the query for an element in the first table of the query, with the values of the query
        /// </summary>
        /// <returns>Update query</returns>
        public string PrepareUpdate()
        {
            //Checks if there is added any tables and if the number of columns and values are the same, to ensure success
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                //Create the query string
                _query.Append("UPDATE " + Tables[0].Name);

                string columnValuePairs;
                int length = Columns.Count;

                columnValuePairs = (new Statement(Columns[0], Values[0])).Render();

                for (int i = 1; i < length; i++)
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

        /// <summary>
        /// Creates a query to delete an element in the table of the query
        /// </summary>
        /// <returns>Delete query</returns>
        public string PrepareDelete()
        {
            //Checks if there is added any tables and if the number of columns and values are the same, to ensure success
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                //Create the query string
                _query.Append("DELETE FROM " + Tables[0].Name);
                if (WhereStatements.Count > 0)
                {
                    _query.Append(" WHERE " + string.Join(" AND ", from item in WhereStatements select item.Render()));
                }
                return _query.ToString();
            }
            return "";
        }
        
        public void Dispose()
        {
        }
    }
}