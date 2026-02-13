using Deal.DeskOne.Infrastructure.Abstractions;
using Npgsql;
using System.Data;

namespace Deal.DeskOne.Infrastructure.Data
{
    public class DapperContext(string connectionString) : IDapperContext
    {
        public IDbConnection CreateConnection()
            => new NpgsqlConnection(connectionString);
    }
}
