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

    public IReadOnlyList<FoodEntry> WeeklyEntries { get; set; } = Array.Empty<FoodEntry>();
    public DateOnly WeekStart { get; set; }
    public DateOnly WeekEnd { get; set; }
    public string? SignedInUserDisplayName { get; set; }
}
