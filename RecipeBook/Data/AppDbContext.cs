using Microsoft.EntityFrameworkCore;
using RecipeBook.Models;
using System.IO;

namespace RecipeBook.Data;

public class AppDbContext : DbContext
{
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Difficulty> Difficulties { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<RecipeStep> RecipeSteps { get; set; } = null!;
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;
    public DbSet<RecipeCategory> RecipeCategories { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<CookedRecord> CookedRecords { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db-server.txt");
        if (!File.Exists(file))
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=RecipeBookDb;Trusted_Connection=True;TrustServerCertificate=True;");
            return;
        }

        var content = File.ReadAllText(file).Trim();
        var connection = content.Contains('=')
            ? content
            : $"Server={content};Database=RecipeBookDb;Trusted_Connection=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().Property(r => r.Name).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(u => u.Login).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(255);
        modelBuilder.Entity<User>().Property(u => u.DisplayName).HasMaxLength(100);
        modelBuilder.Entity<Difficulty>().Property(d => d.DifficultyName).HasMaxLength(20);
        modelBuilder.Entity<Category>().Property(c => c.CategoryName).HasMaxLength(50);
        modelBuilder.Entity<Ingredient>().Property(i => i.IngredientName).HasMaxLength(50);
        modelBuilder.Entity<Recipe>().Property(r => r.Title).HasMaxLength(50);
        modelBuilder.Entity<Recipe>().Property(r => r.Description).HasMaxLength(200);
        modelBuilder.Entity<Recipe>().Property(r => r.ImagePath).HasMaxLength(255);
        modelBuilder.Entity<RecipeIngredient>().Property(ri => ri.Amount).HasPrecision(10, 2);
        modelBuilder.Entity<RecipeIngredient>().Property(ri => ri.Unit).HasMaxLength(20);
        modelBuilder.Entity<Comment>().Property(c => c.CommentText).HasMaxLength(250);
        modelBuilder.Entity<CookedRecord>().Property(cr => cr.PersonalNote).HasMaxLength(200);

        modelBuilder.Entity<RecipeCategory>()
            .HasKey(rc => new { rc.RecipeID, rc.CategoryID });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role).WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Author).WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AuthorID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Difficulty).WithMany(d => d.Recipes)
            .HasForeignKey(r => r.DifficultyID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecipeStep>()
            .HasOne(s => s.Recipe).WithMany(r => r.Steps)
            .HasForeignKey(s => s.RecipeID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe).WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient).WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecipeCategory>()
            .HasOne(rc => rc.Recipe).WithMany(r => r.RecipeCategories)
            .HasForeignKey(rc => rc.RecipeID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeCategory>()
            .HasOne(rc => rc.Category).WithMany(c => c.RecipeCategories)
            .HasForeignKey(rc => rc.CategoryID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Recipe).WithMany(r => r.Comments)
            .HasForeignKey(c => c.RecipeID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User).WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CookedRecord>()
            .HasOne(cr => cr.Recipe).WithMany(r => r.CookedRecords)
            .HasForeignKey(cr => cr.RecipeID).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CookedRecord>()
            .HasOne(cr => cr.User).WithMany(u => u.CookedRecords)
            .HasForeignKey(cr => cr.UserID).OnDelete(DeleteBehavior.Restrict);
    }
}
