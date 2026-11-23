namespace RecipeBook.Models;

public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public List<RecipeCategory> RecipeCategories { get; set; } = new();
}