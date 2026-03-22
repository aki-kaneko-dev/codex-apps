using System.Security.Claims;
using FoodCalendar.Models;
using FoodCalendar.Services;
using FoodCalendar.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodCalendar.Controllers;

[Authorize]
[Route("entries")]
public sealed class EntriesController : Controller
{
    private readonly FoodEntryRepository _repository;

    public EntriesController(FoodEntryRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FoodCalendarViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Home");
        }

        var entry = new FoodEntry
        {
            UserObjectId = GetRequiredUserObjectId(),
            UserPrincipalName = User.FindFirstValue(ClaimTypes.Upn)
                ?? User.FindFirstValue("preferred_username")
                ?? User.Identity?.Name,
            EntryDate = model.EntryDate,
            FoodName = model.FoodName.Trim(),
            Amount = model.Amount.Trim()
        };

        await _repository.CreateEntryAsync(entry, cancellationToken);
        TempData["StatusMessage"] = "食事データを登録しました。";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("week")]
    public async Task<IActionResult> Week(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var weekStart = today.AddDays(-1 * ((7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7));
        var entries = await _repository.GetWeekEntriesAsync(GetRequiredUserObjectId(), weekStart, cancellationToken);
        return PartialView("_WeeklyEntries", entries);
    }

    private string GetRequiredUserObjectId()
    {
        return User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? User.FindFirstValue("oid")
            ?? throw new InvalidOperationException("The signed-in user does not include an Entra object id claim.");
    }
}
