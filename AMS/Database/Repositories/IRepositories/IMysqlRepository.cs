using MySql.Data.MySqlClient;

namespace Asset_Management_System.Database.Repositories
{
    public interface IMysqlRepository<T> : IRepository<T>
    {
        T DBOToModelConvert(MySqlDataReader reader);
    }
}
