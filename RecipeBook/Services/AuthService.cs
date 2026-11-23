using Microsoft.EntityFrameworkCore;
using RecipeBook.Data;
using RecipeBook.Models;

namespace RecipeBook.Services;

public static class Session
{
    public static User? CurrentUser { get; private set; }
    public static void Set(User? user) => CurrentUser = user;
    public static bool IsLogged => CurrentUser != null;
    public static bool IsAdmin => CurrentUser?.Role?.Name == "Администратор";
}

public class AuthService
{
    public string? Login(string login, string password)
    {
        try
        {
            using var db = new AppDbContext();
            var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return "Неверный логин или пароль";

            Session.Set(user);
            return null;
        }
        catch (Exception)
        {
            return "Не удалось подключиться к базе данных. Проверьте соединение и попробуйте снова";
        }
    }

    public string? Register(string login, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            return "Заполните логин и пароль";

        try
        {
            using var db = new AppDbContext();
            if (db.Users.Any(u => u.Login == login))
                return "Пользователь с таким логином уже существует";

            var user = new User
            {
                Login = login.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? login.Trim() : displayName.Trim(),
                RoleID = db.Roles.First(r => r.Name == "Пользователь").RoleID
            };
            db.Users.Add(user);
            db.SaveChanges();

            Session.Set(user);
            return null;
        }
        catch (Exception)
        {
            return "Не удалось подключиться к базе данных. Проверьте соединение и попробуйте снова";
        }
    }

    public void Logout() => Session.Set(null);
}