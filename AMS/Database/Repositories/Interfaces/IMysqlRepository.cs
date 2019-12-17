using MySql.Data.MySqlClient;

namespace AMS.Database.Repositories.Interfaces
{
    public interface IMySqlRepository<T> : IRepository<T>
    {
        T DataMapper(MySqlDataReader reader);
    }
}
