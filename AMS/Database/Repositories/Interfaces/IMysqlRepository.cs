using MySql.Data.MySqlClient;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IMysqlRepository<T> : IRepository<T>
    {
        T DBOToModelConvert(MySqlDataReader reader);
    }
}
