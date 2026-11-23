namespace RecipeBook.Models;

public class Difficulty
{
    public int DifficultyID { get; set; }
    public string DifficultyName { get; set; } = string.Empty;

    public List<Recipe> Recipes { get; set; } = new();
}