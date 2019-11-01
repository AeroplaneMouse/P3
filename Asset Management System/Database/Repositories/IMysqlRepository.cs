using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    interface IMysqlRepository<T> : IRepository<T>
    {
        T DBOToModelConvert(MySqlDataReader reader);
    }
}
