using Dapper;
using FoodCalendar.Models;

namespace FoodCalendar.Services;

public sealed class FoodEntryRepository
{
    private sealed class FoodEntryRow
    {
        public int Id { get; init; }
        public string UserObjectId { get; init; } = string.Empty;
        public string? UserPrincipalName { get; init; }
        public DateTime EntryDate { get; init; }
        public string FoodName { get; init; } = string.Empty;
        public string Amount { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

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
        var rows = await connection.QueryAsync<FoodEntryRow>(new CommandDefinition(sql, new
        {
            UserObjectId = userObjectId,
            WeekStart = weekStart.ToDateTime(TimeOnly.MinValue)
        }, cancellationToken: cancellationToken));

        return rows.Select(row => new FoodEntry
        {
            Id = row.Id,
            UserObjectId = row.UserObjectId,
            UserPrincipalName = row.UserPrincipalName,
            EntryDate = DateOnly.FromDateTime(row.EntryDate),
            FoodName = row.FoodName,
            Amount = row.Amount,
            CreatedAt = row.CreatedAt
        }).ToList();
    }
}