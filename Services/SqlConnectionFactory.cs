using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;

namespace FoodCalendar.Services;

public sealed class SqlConnectionFactory
{
    private static readonly TokenRequestContext TokenRequestContext = new(["https://database.windows.net/.default"]);
    private readonly string _connectionString;
    private readonly TokenCredential _credential = new DefaultAzureCredential();

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("SqlServer connection string not found.");
    }

    public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await _credential.GetTokenAsync(TokenRequestContext, cancellationToken);

        var connection = new SqlConnection(_connectionString)
        {
            AccessToken = accessToken.Token
        };

        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
