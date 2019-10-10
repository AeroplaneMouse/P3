using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Database;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    public class LogRepository : ILogRepository<Log>
    {
        private DBConnection dbcon;

        public LogRepository()
        {
            this.dbcon = DBConnection.Instance();
        }
        
        public bool Insert(Log entity)
        {
            bool query_success = false;

            if (dbcon.IsConnect())
            {
                try
                {
                    const string query = "INSERT INTO logs (label, color, options, department_id, parent_id) " +
                                         "VALUES (@label, @color, @options, @department_id, @parent_id)";

                    using (var cmd = new MySqlCommand(query, dbcon.Connection))
                    {
                        cmd.Parameters.Add("@label", MySqlDbType.String);
                        cmd.Parameters["@label"].Value = entity;

                        cmd.Parameters.Add("@color", MySqlDbType.String);
                        cmd.Parameters["@color"].Value = entity.Color;

                        cmd.Parameters.Add("@options", MySqlDbType.JSON);
                        cmd.Parameters["@options"].Value = entity.SerializedFields;

                        cmd.Parameters.Add("@department_id", MySqlDbType.UInt64);
                        cmd.Parameters["@department_id"].Value = entity.DepartmentID;

                        cmd.Parameters.Add("@parent_id", MySqlDbType.UInt64);
                        cmd.Parameters["@parent_id"].Value = entity.ParentID;

                        query_success = cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (MySqlException e)
                {

                }
                finally
                {
                    dbcon.Close();
                }
            }

            return query_success;
        }

        public bool GetLogEntries(Log entity)
        {
            throw new System.NotImplementedException();
        }
    }
}