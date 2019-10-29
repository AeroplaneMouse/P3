using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database
{
    public class MySqlHandler
    {

        public bool RawQuery(string raw_query, MySqlParameterCollection par = null)
        {
            DBConnection dbcon = DBConnection.Instance();
            bool result = false;

            try
            {
                if (dbcon.IsConnect())
                {
                    using (var cmd = new MySqlCommand(raw_query, dbcon.Connection))
                    {
                        if (par != null)
                        {
                            foreach (var param in par)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }

                        result = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                dbcon.Close();
            }
            
            return result;
        }
    }
}