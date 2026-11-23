namespace RecipeBook.Models;

public class RecipeCategory
{
    public int RecipeID { get; set; }
    public int CategoryID { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public Category Category { get; set; } = null!;
}