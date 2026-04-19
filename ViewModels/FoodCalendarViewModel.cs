using System.ComponentModel.DataAnnotations;
using FoodCalendar.Models;

namespace FoodCalendar.ViewModels;

public sealed class FoodCalendarViewModel
{
    [DataType(DataType.Date)]
    public DateOnly EntryDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    [Required]
    [StringLength(200)]
    [Display(Name = "食べ物名")]
    public string FoodName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "量")]
    public string Amount { get; set; } = string.Empty;

    [Range(1, 5000)]
    [Display(Name = "摂取カロリー (kcal)")]
    public int Calories { get; set; } = 600;

    [Range(1, 5000)]
    [Display(Name = "1食の目標カロリー (kcal)")]
    public int TargetCalories { get; set; } = 650;

    public IReadOnlyList<FoodEntry> WeeklyEntries { get; set; } = Array.Empty<FoodEntry>();
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public string? SignedInUserDisplayName { get; set; }
}
