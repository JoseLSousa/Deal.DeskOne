using System.Data;

namespace Deal.DeskOne.Infrastructure.Abstractions
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
    }
}
