namespace RecipeBook.Models;

public class Comment
{
    public int CommentID { get; set; }
    public int RecipeID { get; set; }
    public int UserID { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; }

    public Recipe Recipe { get; set; } = null!;
    public User User { get; set; } = null!;
}