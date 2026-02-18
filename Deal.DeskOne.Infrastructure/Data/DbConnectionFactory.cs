using Deal.DeskOne.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Deal.DeskOne.Infrastructure.Data
{
    internal class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        public IDbConnection CreateConnection()
            => new NpgsqlConnection(configuration.GetConnectionString("Postgres"));
    }
}
