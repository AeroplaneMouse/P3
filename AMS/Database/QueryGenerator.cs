using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AMS.Database
{
    public class QueryGenerator
    {
        public List<string> Columns;
        public List<List<string>> Values;
        public List<Table> Tables;
        public List<Statement> WhereStatements;
        public List<Statement> HavingStatements;
        private Dictionary<string, string> _orderBys;
        public string GroupBy { get; set; }
        public int Limit { get; set; } = 0;
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
            if (!_orderBys.ContainsKey(column))
            {
                _orderBys.Add(column, (ascending ? "ASC" : "DESC"));
            }
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
            
            if (!string.IsNullOrEmpty(GroupBy))
            {
                _query.Append(" GROUP BY " + GroupBy);
            }
            
            if (HavingStatements.Count > 0)
            {
                _query.Append(" HAVING " + string.Join(" AND ", from item in HavingStatements select item.Render()));
            }
            
            if (_orderBys.Count > 0)
            {
                _query.Append($" ORDER BY {_orderBys.First().Key} {_orderBys.First().Value}");
            }

            if (Limit > 0)
            {
                _query.Append(" LIMIT " + Limit);
            }

            return _query.ToString();
        }

        public void Reset()
        {
            WhereStatements.Clear();
            HavingStatements.Clear();
            Tables.Clear();
            Columns.Clear();
            Values.Clear();
            _orderBys.Clear();
        }
    }
}