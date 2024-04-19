using System.Data;

namespace Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();
}
