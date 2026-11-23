using Microsoft.EntityFrameworkCore;
using RecipeBook.Models;

namespace RecipeBook.Data;

public static class DbInitializer
{
    public static void Initialize()
    {
        using var db = new AppDbContext();

        db.Database.Migrate();

        if (!db.Roles.Any())
        {
            db.Roles.AddRange(
                new Role { Name = "Пользователь" },
                new Role { Name = "Администратор" });
            db.SaveChanges();
        }

        if (!db.Difficulties.Any())
        {
            db.Difficulties.AddRange(
                new Difficulty { DifficultyName = "Не указана" },
                new Difficulty { DifficultyName = "Легко" },
                new Difficulty { DifficultyName = "Средне" },
                new Difficulty { DifficultyName = "Сложно" });
            db.SaveChanges();
        }

        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { CategoryName = "Супы" },
                new Category { CategoryName = "Салаты" },
                new Category { CategoryName = "Горячее" },
                new Category { CategoryName = "Выпечка" },
                new Category { CategoryName = "Десерты" },
                new Category { CategoryName = "Напитки" });
            db.SaveChanges();
        }

        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                DisplayName = "Администратор",
                RoleID = db.Roles.First(r => r.Name == "Администратор").RoleID
            });
            db.SaveChanges();
        }
    }
}