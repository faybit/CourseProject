namespace RecipeBook.Models;

public class RecipeIngredient
{
    public int RecipeIngredientID { get; set; }
    public int RecipeID { get; set; }
    public int IngredientID { get; set; }
    public decimal Amount { get; set; }
    public string? Unit { get; set; }
    public int SortOrder { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}