namespace RecipeBook.Models;

public class Ingredient
{
    public int IngredientID { get; set; }
    public string IngredientName { get; set; } = string.Empty;

    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
}