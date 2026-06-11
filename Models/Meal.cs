namespace MealTimer.Models;

public class Meal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<FoodItem> FoodItems { get; set; } = new();
}
