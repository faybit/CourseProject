using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using RecipeBook.Models;
using RecipeBook.Services;
using System.Collections.ObjectModel;

namespace RecipeBook.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly RecipeService _recipes = new();

    [ObservableProperty]
    private ObservableCollection<Recipe> _recipesList = new();

    [ObservableProperty]
    private string _search = string.Empty;

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private Difficulty? _selectedDifficulty;

    [ObservableProperty]
    private string _selectedSort = "По новизне";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<Difficulty> Difficulties { get; } = new();
    public List<string> SortVariants { get; } = new()
        { "По новизне", "По названию", "По времени приготовления" };

    public MainViewModel()
    {
        LoadFilters();
        LoadRecipes();
        RefreshSession();
    }

    public void LoadFilters()
    {
        try
        {
            Categories.Clear();
            Categories.Add(new Category { CategoryID = 0, CategoryName = "Все категории" });
            foreach (var c in _recipes.GetCategories())
                Categories.Add(c);

            Difficulties.Clear();
            Difficulties.Add(new Difficulty { DifficultyID = 0, DifficultyName = "Любая сложность" });
            foreach (var d in _recipes.GetDifficulties())
                Difficulties.Add(d);

            SelectedCategory = Categories[0];
            SelectedDifficulty = Difficulties[0];
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить справочники: " + ex.Message;
        }
    }

    [ObservableProperty]
    private bool _isAdmin;

    [ObservableProperty]
    private bool _isLogged;

    public void RefreshSession()
    {
        IsAdmin = Session.IsAdmin;
        IsLogged = Session.IsLogged;
    }

    [RelayCommand]
    public void LoadRecipes()
    {
        try
        {
            var list = _recipes.GetRecipes(
                Search,
                SelectedCategory?.CategoryID == 0 ? null : SelectedCategory?.CategoryID,
                SelectedDifficulty?.DifficultyID == 0 ? null : SelectedDifficulty?.DifficultyID,
                SelectedSort);

            RecipesList = new ObservableCollection<Recipe>(list);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            RecipesList = new ObservableCollection<Recipe>();
            ErrorMessage = "Не удалось загрузить рецепты: " + ex.Message;
        }
    }

    partial void OnSearchChanged(string value) => LoadRecipes();
    partial void OnSelectedCategoryChanged(Category? value) => LoadRecipes();
    partial void OnSelectedDifficultyChanged(Difficulty? value) => LoadRecipes();
    partial void OnSelectedSortChanged(string value) => LoadRecipes();
}