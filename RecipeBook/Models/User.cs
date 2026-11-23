using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeBook.Models;

public class User
{
    public int UserID { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int RoleID { get; set; }

    public Role Role { get; set; } = null!;
    public List<Recipe> Recipes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<CookedRecord> CookedRecords { get; set; } = new();
}