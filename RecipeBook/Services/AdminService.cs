using Microsoft.EntityFrameworkCore;
using RecipeBook.Data;
using RecipeBook.Models;

namespace RecipeBook.Services;

public class AdminService
{
    public string? AddCategory(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Введите название категории";
        using var db = new AppDbContext();
        if (db.Categories.Any(c => c.CategoryName == name.Trim()))
            return "Такая категория уже есть";
        db.Categories.Add(new Category { CategoryName = name.Trim() });
        db.SaveChanges();
        return null;
    }

    public string? AddDifficulty(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Введите название сложности";
        using var db = new AppDbContext();
        if (db.Difficulties.Any(d => d.DifficultyName == name.Trim()))
            return "Такая сложность уже есть";
        db.Difficulties.Add(new Difficulty { DifficultyName = name.Trim() });
        db.SaveChanges();
        return null;
    }

    public string? AddIngredient(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Введите название ингредиента";
        using var db = new AppDbContext();
        if (db.Ingredients.Any(i => i.IngredientName == name.Trim()))
            return "Такой ингредиент уже есть";
        db.Ingredients.Add(new Ingredient { IngredientName = name.Trim() });
        db.SaveChanges();
        return null;
    }

    public string? DeleteCategory(int id)
    {
        try
        {
            using var db = new AppDbContext();
            db.Categories.Remove(db.Categories.First(c => c.CategoryID == id));
            db.SaveChanges();
            return null;
        }
        catch (DbUpdateException)
        {
            return "Нельзя удалить категорию: она используется в рецептах";
        }
        catch (Exception ex)
        {
            return "Не удалось удалить категорию: " + ex.Message;
        }
    }

    public string? DeleteDifficulty(int id)
    {
        try
        {
            using var db = new AppDbContext();
            db.Difficulties.Remove(db.Difficulties.First(d => d.DifficultyID == id));
            db.SaveChanges();
            return null;
        }
        catch (DbUpdateException)
        {
            return "Нельзя удалить сложность: она используется в рецептах";
        }
        catch (Exception ex)
        {
            return "Не удалось удалить сложность: " + ex.Message;
        }
    }

    public string? DeleteIngredient(int id)
    {
        try
        {
            using var db = new AppDbContext();
            db.Ingredients.Remove(db.Ingredients.First(i => i.IngredientID == id));
            db.SaveChanges();
            return null;
        }
        catch (DbUpdateException)
        {
            return "Нельзя удалить ингредиент: он используется в рецептах";
        }
        catch (Exception ex)
        {
            return "Не удалось удалить ингредиент: " + ex.Message;
        }
    }

    public string? DeleteComment(int commentId)
    {
        try
        {
            using var db = new AppDbContext();
            db.Comments.Remove(db.Comments.First(c => c.CommentID == commentId));
            db.SaveChanges();
            return null;
        }
        catch (Exception ex)
        {
            return "Не удалось удалить комментарий: " + ex.Message;
        }
    }
}