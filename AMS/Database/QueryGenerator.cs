using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AMS.Database
{
    public class QueryGenerator : IDisposable
    {
        public List<string> Columns;
        public List<List<string>> Values;
        public List<Table> Tables;
        public List<Statement> WhereStatements;
        public List<Statement> HavingStatements;
        private Dictionary<string, string> _orderBys;
        public string GroupBy { get; set; }
        private StringBuilder _query;

        public QueryGenerator()
        {
            Columns = new List<string>();
            Values = new List<List<string>>{new List<string>()};
            Tables = new List<Table>();
            WhereStatements = new List<Statement>();
            HavingStatements = new List<Statement>();
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
            _query.Clear();
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

            if (!string.IsNullOrEmpty(GroupBy))
            {
                _query.Append(" GROUP BY " + GroupBy);
            }

            if (HavingStatements.Count > 0)
            {
                _query.Append(" HAVING " + string.Join(" AND ", from item in HavingStatements select item.Render()));
            }
            
            return _query.ToString();
        }

        /// <summary>
        /// Creates a query to insert a new row into the first table of the query, with the values of the query
        /// </summary>
        /// <returns>Insert query</returns>
        public string PrepareInsert()
        {
            _query.Clear();
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                _query.Append("INSERT INTO " + Tables[0].Name);
                _query.Append(" ( " + GetColumns() + " ) VALUES ");

                List<String> objects = new List<string>();

                foreach (var item in Values)
                {
                    objects.Add("(" + string.Join(", ", item) + ")");
                }

                _query.Append(string.Join(",", objects));
                
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
            /*
            _query.Clear();
            //Checks if there is added any tables and if the number of columns and values are the same, to ensure success
            if (Tables.Count > 0 && Columns.Count == Values.Count)
            {
                //Create the query string
                _query.Append("UPDATE " + Tables[0].Name+ " SET ");
                int counter = Tables.Count;
                

                    _query.Append(string.Join(", ", Values.ForEach() new Statement(Columns[i], Values[i]).Render());
                     columnValuePairs += $", {(new Statement(Columns[i], Values[i])).Render()}";
                

                _query.Append(" SET " + columnValuePairs);
                
                if (WhereStatements.Count > 0)
                {
                    _query.Append(" WHERE " + string.Join(" AND ", from item in WhereStatements select item.Render()));
                }

                return _query.ToString();
            }

            return "";
            */
            return null;
        }
        
        /// <summary>
        /// Creates a query to delete an element in the table of the query
        /// </summary>
        /// <returns>Delete query</returns>
        public string PrepareDelete()
        {
            _query.Clear();
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

        private string prepareValue(string value)
        {
            if (int.TryParse(value, out int returnedInt))
            {
                return value;
            }

            return "'" + value + "'";
        }

        public void Reset()
        {
            WhereStatements.Clear();
            HavingStatements.Clear();
            Tables.Clear();
            Columns.Clear();
            Values.Clear();
        }
    }
}