namespace RecipeBook.Models;

public class RecipeStep
{
    public int RecipeStepID { get; set; }
    public int RecipeID { get; set; }
    public int StepNumber { get; set; }
    public string Text { get; set; } = string.Empty;

    public Recipe Recipe { get; set; } = null!;
}