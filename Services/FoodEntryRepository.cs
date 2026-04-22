using Dapper;
using FoodCalendar.Models;

namespace FoodCalendar.Services;

public sealed class FoodEntryRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public FoodEntryRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateEntryAsync(FoodEntry entry, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO dbo.FoodEntries (UserObjectId, UserPrincipalName, EntryDate, FoodName, Amount, Calories, TargetCalories)
            VALUES (@UserObjectId, @UserPrincipalName, @EntryDate, @FoodName, @Amount, @Calories, @TargetCalories);
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            entry.UserObjectId,
            entry.UserPrincipalName,
            EntryDate = entry.EntryDate.ToDateTime(TimeOnly.MinValue),
            entry.FoodName,
            entry.Amount,
            entry.Calories,
            entry.TargetCalories
        }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<FoodEntry>> GetWeekEntriesAsync(string userObjectId, DateOnly weekStart, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT Id,
                   UserObjectId,
                   UserPrincipalName,
                   EntryDate,
                   FoodName,
                   Amount,
                   Calories,
                   TargetCalories,
                   CreatedAt
            FROM dbo.FoodEntries
            WHERE UserObjectId = @UserObjectId
              AND EntryDate >= @WeekStart
              AND EntryDate < DATEADD(DAY, 7, @WeekStart)
            ORDER BY EntryDate ASC, Id ASC;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<FoodEntry>(new CommandDefinition(sql, new
        {
            UserObjectId = userObjectId,
            WeekStart = weekStart.ToDateTime(TimeOnly.MinValue)
        }, cancellationToken: cancellationToken));

        return rows.ToList();
    }
}
