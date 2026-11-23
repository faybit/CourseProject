namespace RecipeBook.Models;

public class Recipe
{
    public int RecipeID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CookinTime { get; set; }
    public int Servings { get; set; }
    public string? ImagePath { get; set; }
    public int DifficultyID { get; set; }
    public int AuthorID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }

    public Difficulty Difficulty { get; set; } = null!;
    public User Author { get; set; } = null!;
    public List<RecipeStep> Steps { get; set; } = new();
    public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    public List<RecipeCategory> RecipeCategories { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<CookedRecord> CookedRecords { get; set; } = new();
}