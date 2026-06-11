using MealTimer.Models;

namespace MealTimer.Services;

public class MealStateService
{
    public Meal ActiveMeal { get; set; } = new() { Name = "My Meal" };

    // ── Timer state ──────────────────────────────────────────────────────────
    public bool IsTimerStarted { get; set; } = false;
    public bool IsTimerRunning { get; set; } = false;
    public int ElapsedSeconds { get; set; } = 0;
    public bool IsMealComplete { get; set; } = false;

    // Tracks which phase-change beeps have already fired (prevent re-triggering)
    public HashSet<Guid> PrepAlertsSent { get; } = new();
    public HashSet<Guid> CookAlertsSent { get; } = new();

    // ── Derived ──────────────────────────────────────────────────────────────

    /// <summary>Overall meal duration in seconds (equals the longest TotalTimeMinutes * 60).</summary>
    public int MealTotalSeconds =>
        ActiveMeal.FoodItems.Count > 0
            ? ActiveMeal.FoodItems.Max(f => f.TotalTimeMinutes) * 60
            : 0;

    public int RemainingSeconds => Math.Max(0, MealTotalSeconds - ElapsedSeconds);

    // ── State mutations ───────────────────────────────────────────────────────

    public void ResetCooking()
    {
        IsTimerStarted = false;
        IsTimerRunning = false;
        ElapsedSeconds = 0;
        IsMealComplete = false;
        PrepAlertsSent.Clear();
        CookAlertsSent.Clear();
    }

    // ── Scheduling ────────────────────────────────────────────────────────────

    /// <summary>
    /// Calculate the start offsets (in seconds from "Start") for an item.
    /// StartOffset = (MealTotalMinutes - item.TotalTimeMinutes) * 60
    /// PrepStartOffset = StartOffset  (if prep time exists)
    /// CookStartOffset = StartOffset + PrepTimeMinutes * 60  (or StartOffset if no prep)
    /// DoneOffset      = StartOffset + TotalTimeMinutes * 60
    /// </summary>
    public FoodItemSchedule GetSchedule(FoodItem item)
    {
        int mealTotalMinutes = MealTotalSeconds / 60;
        int startOffsetSeconds = (mealTotalMinutes - item.TotalTimeMinutes) * 60;

        int? prepStartOffset = item.PrepTimeMinutes.HasValue ? startOffsetSeconds : null;
        int cookStartOffset = item.PrepTimeMinutes.HasValue
            ? startOffsetSeconds + item.PrepTimeMinutes.Value * 60
            : startOffsetSeconds;
        int doneOffset = startOffsetSeconds + item.TotalTimeMinutes * 60;

        return new FoodItemSchedule
        {
            PrepStartOffsetSeconds = prepStartOffset,
            CookStartOffsetSeconds = cookStartOffset,
            DoneOffsetSeconds = doneOffset
        };
    }

    /// <summary>Determine the current status of a food item based on elapsed time.</summary>
    public FoodItemStatus GetItemStatus(FoodItem item)
    {
        if (!IsTimerStarted) return FoodItemStatus.Waiting;

        var schedule = GetSchedule(item);

        if (ElapsedSeconds >= schedule.DoneOffsetSeconds)
            return FoodItemStatus.Ready;

        if (ElapsedSeconds >= schedule.CookStartOffsetSeconds)
            return FoodItemStatus.Cooking;

        if (schedule.PrepStartOffsetSeconds.HasValue
            && ElapsedSeconds >= schedule.PrepStartOffsetSeconds.Value)
            return FoodItemStatus.Preparing;

        return FoodItemStatus.Waiting;
    }
}

public record FoodItemSchedule
{
    public int? PrepStartOffsetSeconds { get; init; }
    public int CookStartOffsetSeconds { get; init; }
    public int DoneOffsetSeconds { get; init; }
}

public enum FoodItemStatus
{
    Waiting,
    Preparing,
    Cooking,
    Ready
}
