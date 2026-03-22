namespace FoodCalendar.Models;

public sealed class FoodEntry
{
    public int Id { get; init; }
    public string UserObjectId { get; init; } = string.Empty;
    public string? UserPrincipalName { get; init; }
    public DateOnly EntryDate { get; init; }
    public string FoodName { get; init; } = string.Empty;
    public string Amount { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
