using Microsoft.EntityFrameworkCore;
using RecipeBook.Data;
using RecipeBook.Models;

namespace RecipeBook.Services;

public class RecipeService
{
    public List<Recipe> GetRecipes(string search = "", int? categoryId = null,
        int? difficultyId = null, string sortBy = "По новизне")
    {
        using var db = new AppDbContext();

        IQueryable<Recipe> query = db.Recipes
            .Include(r => r.Difficulty)
            .Include(r => r.Author)
            .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Title.Contains(search));

        if (categoryId != null)
            query = query.Where(r => r.RecipeCategories.Any(rc => rc.CategoryID == categoryId));

        if (difficultyId != null)
            query = query.Where(r => r.DifficultyID == difficultyId);

        query = sortBy switch
        {
            "По названию" => query.OrderBy(r => r.Title),
            "По времени приготовления" => query.OrderBy(r => r.CookinTime),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        return query.ToList();
    }

    public List<Category> GetCategories()
    {
        using var db = new AppDbContext();
        return db.Categories.OrderBy(c => c.CategoryName).ToList();
    }

    public List<Difficulty> GetDifficulties()
    {
        using var db = new AppDbContext();
        return db.Difficulties.OrderBy(d => d.DifficultyID).ToList();
    }

    public Recipe? GetRecipe(int recipeId)
    {
        using var db = new AppDbContext();
        var recipe = db.Recipes
            .Include(r => r.Difficulty)
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.Comments).ThenInclude(c => c.User)
            .FirstOrDefault(r => r.RecipeID == recipeId);

        if (recipe == null)
            return null;

        recipe.Steps = recipe.Steps.OrderBy(s => s.StepNumber).ToList();
        recipe.RecipeIngredients = recipe.RecipeIngredients.OrderBy(ri => ri.SortOrder).ToList();
        recipe.Comments = recipe.Comments.OrderByDescending(c => c.PublicationDate).ToList();
        return recipe;
    }

    public bool IsCooked(int recipeId, int userId)
    {
        using var db = new AppDbContext();
        return db.CookedRecords.Any(cr => cr.RecipeID == recipeId && cr.UserID == userId);
    }

    public void ToggleCooked(int recipeId, int userId)
    {
        using var db = new AppDbContext();
        var existing = db.CookedRecords
            .FirstOrDefault(cr => cr.RecipeID == recipeId && cr.UserID == userId);

        if (existing != null)
            db.CookedRecords.Remove(existing);
        else
            db.CookedRecords.Add(new CookedRecord
            {
                RecipeID = recipeId,
                UserID = userId,
                CookedAt = DateTime.Now
            });

        db.SaveChanges();
    }

    public void AddComment(int recipeId, int userId, string text)
    {
        using var db = new AppDbContext();
        db.Comments.Add(new Comment
        {
            RecipeID = recipeId,
            UserID = userId,
            CommentText = text,
            PublicationDate = DateTime.Now
        });
        db.SaveChanges();
    }

    public List<Ingredient> GetIngredients()
    {
        using var db = new AppDbContext();
        return db.Ingredients.OrderBy(i => i.IngredientName).ToList();
    }

    public Recipe? GetForEdit(int recipeId)
    {
        using var db = new AppDbContext();
        var recipe = db.Recipes
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients).ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
            .FirstOrDefault(r => r.RecipeID == recipeId);

        if (recipe == null)
            return null;

        recipe.Steps = recipe.Steps.OrderBy(s => s.StepNumber).ToList();
        recipe.RecipeIngredients = recipe.RecipeIngredients.OrderBy(ri => ri.SortOrder).ToList();
        return recipe;
    }

    public void SaveRecipe(Recipe recipe)
    {
        using var db = new AppDbContext();
        if (recipe.RecipeID == 0)
        {
            db.Recipes.Add(recipe); 
        }
        else
        {
            var existing = db.Recipes
                .Include(r => r.Steps)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.RecipeCategories)
                .First(r => r.RecipeID == recipe.RecipeID);

            db.RecipeSteps.RemoveRange(existing.Steps);
            db.RecipeIngredients.RemoveRange(existing.RecipeIngredients);
            db.RecipeCategories.RemoveRange(existing.RecipeCategories);

            existing.Title = recipe.Title;
            existing.Description = recipe.Description;
            existing.CookinTime = recipe.CookinTime;
            existing.Servings = recipe.Servings;
            existing.ImagePath = recipe.ImagePath;
            existing.DifficultyID = recipe.DifficultyID;
            existing.UpdateAt = DateTime.Now;

            existing.Steps = recipe.Steps;
            existing.RecipeIngredients = recipe.RecipeIngredients;
            existing.RecipeCategories = recipe.RecipeCategories;
        }
        db.SaveChanges();
    }

    public void DeleteRecipe(int recipeId)
    {
        using var db = new AppDbContext();
        var recipe = db.Recipes.First(r => r.RecipeID == recipeId);
        db.Recipes.Remove(recipe); 
        db.SaveChanges();
    }

    public List<CookedRecord> GetCooked(int userId)
    {
        using var db = new AppDbContext();
        return db.CookedRecords
            .Include(cr => cr.Recipe).ThenInclude(r => r.Difficulty)
            .Where(cr => cr.UserID == userId)
            .OrderByDescending(cr => cr.CookedAt)
            .ToList();
    }
}