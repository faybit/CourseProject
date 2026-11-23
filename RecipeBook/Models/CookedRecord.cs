namespace RecipeBook.Models;

public class CookedRecord
{
    public int CookedRecordID { get; set; }
    public int RecipeID { get; set; }
    public int UserID { get; set; }
    public DateTime CookedAt { get; set; }
    public string? PersonalNote { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public User User { get; set; } = null!;
}