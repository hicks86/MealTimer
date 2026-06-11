namespace MealTimer.Models;

public class FoodItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int CookTimeMinutes { get; set; }
    public int? PrepTimeMinutes { get; set; }
    public string CookingDevice { get; set; } = string.Empty;
    public string Temperature { get; set; } = string.Empty;

    /// <summary>Total time from when prep/cook starts until dish is ready.</summary>
    public int TotalTimeMinutes => (PrepTimeMinutes ?? 0) + CookTimeMinutes;
}
