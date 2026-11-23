using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly AdminService _admin = new();
    private readonly RecipeService _recipes = new();

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    private string _newDifficultyName = string.Empty;

    [ObservableProperty]
    private string _newIngredientName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Difficulty> Difficulties { get; } = new();
    public ObservableCollection<Ingredient> Ingredients { get; } = new();

    public AdminViewModel()
    {
        Load();
    }

    public void Load()
    {
        try
        {
            Categories.Clear();
            Difficulties.Clear();
            Ingredients.Clear();
            foreach (var c in _recipes.GetCategories()) Categories.Add(c);
            foreach (var d in _recipes.GetDifficulties()) Difficulties.Add(d);
            foreach (var i in _recipes.GetIngredients()) Ingredients.Add(i);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить справочники: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AddCategory()
    {
        var error = _admin.AddCategory(NewCategoryName);
        if (error != null) { ErrorMessage = error; return; }
        NewCategoryName = string.Empty;
        Load();
    }

    [RelayCommand]
    private void AddDifficulty()
    {
        var error = _admin.AddDifficulty(NewDifficultyName);
        if (error != null) { ErrorMessage = error; return; }
        NewDifficultyName = string.Empty;
        Load();
    }

    [RelayCommand]
    private void AddIngredient()
    {
        var error = _admin.AddIngredient(NewIngredientName);
        if (error != null) { ErrorMessage = error; return; }
        NewIngredientName = string.Empty;
        Load();
    }

    [RelayCommand]
    private void DeleteCategory(Category item)
    {
        var error = _admin.DeleteCategory(item.CategoryID);
        if (error != null) { ErrorMessage = error; return; }
        Load();
    }

    [RelayCommand]
    private void DeleteDifficulty(Difficulty item)
    {
        var error = _admin.DeleteDifficulty(item.DifficultyID);
        if (error != null) { ErrorMessage = error; return; }
        Load();
    }

    [RelayCommand]
    private void DeleteIngredient(Ingredient item)
    {
        var error = _admin.DeleteIngredient(item.IngredientID);
        if (error != null) { ErrorMessage = error; return; }
        Load();
    }
}