namespace FoodCalendar.Models;

public sealed class FoodEntry
{
    public int Id { get; init; }
    public string UserObjectId { get; init; } = string.Empty;
    public string? UserPrincipalName { get; init; }
    public DateOnly EntryDate { get; init; }
    public string FoodName { get; init; } = string.Empty;
    public string Amount { get; init; } = string.Empty;
    public int Calories { get; init; }
    public int TargetCalories { get; init; }
    public DateTime CreatedAt { get; init; }

    public string CalorieTrendSymbol => Calories == TargetCalories
        ? "→"
        : Calories > TargetCalories
            ? "↑"
            : "↓";
}
