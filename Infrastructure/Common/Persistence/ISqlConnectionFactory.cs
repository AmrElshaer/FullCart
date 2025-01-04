
namespace Infrastructure.Common.Persistence;
public interface ISqlConnectionFactory
{
    string GetConnectionString();
}
public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public string GetConnectionString()
    {
        return connectionString;
    }

 
}