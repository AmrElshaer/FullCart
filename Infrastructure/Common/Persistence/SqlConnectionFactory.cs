using System.Data;
using Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Common.Persistence;

public class SqlConnectionFactory : ISqlConnectionFactory, IDisposable
{
    private readonly string _connectionString;
    private IDbConnection _connection = default!;

    public SqlConnectionFactory(string connectionString)
    {
        this._connectionString = connectionString;
    }

    public IDbConnection GetOpenConnection()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (this._connection == null || this._connection.State != ConnectionState.Open)
        {
            this._connection = new SqlConnection(_connectionString);
            this._connection.Open();
        }

        return this._connection;
    }

    public void Dispose()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (this._connection != null && this._connection.State == ConnectionState.Open)
        {
            this._connection.Dispose();
        }
    }
}
