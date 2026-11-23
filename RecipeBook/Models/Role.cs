namespace RecipeBook.Models;

public class Role
{
    public int RoleID { get; set;  }
    public string Name { get; set; } = string.Empty;

    public List<User> Users { get; set; } = new();
}