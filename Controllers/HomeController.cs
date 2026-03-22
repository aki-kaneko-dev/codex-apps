using System.Security.Claims;
using FoodCalendar.Models;
using FoodCalendar.Services;
using FoodCalendar.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodCalendar.Controllers;

[Authorize]
public sealed class HomeController : Controller
{
    private readonly FoodEntryRepository _repository;

    public HomeController(FoodEntryRepository repository)
    {
        _repository = repository;
    }

    [AllowAnonymous]
    public IActionResult Privacy() => View();

    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await BuildViewModelAsync(cancellationToken);
        return View(model);
    }


    private async Task<FoodCalendarViewModel> BuildViewModelAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var weekStart = today.AddDays(-1 * ((7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7));
        var entries = await _repository.GetWeekEntriesAsync(GetRequiredUserObjectId(), weekStart, cancellationToken);

        return new FoodCalendarViewModel
        {
            EntryDate = today,
            FoodName = string.Empty,
            Amount = string.Empty,
            WeeklyEntries = entries,
            WeekStart = weekStart,
            WeekEnd = weekStart.AddDays(6),
            SignedInUserDisplayName = User.Identity?.Name
        };
    }

    private string GetRequiredUserObjectId()
    {
        return User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? User.FindFirstValue("oid")
            ?? throw new InvalidOperationException("The signed-in user does not include an Entra object id claim.");
    }
}
