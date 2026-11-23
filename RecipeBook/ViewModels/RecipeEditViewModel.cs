using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class IngredientRow : ObservableObject
{
    [ObservableProperty]
    private Ingredient? _selectedIngredient;

    [ObservableProperty]
    private string _amount = string.Empty;

    [ObservableProperty]
    private string _unit = string.Empty;
}

public partial class StepRow : ObservableObject
{
    [ObservableProperty]
    private string _text = string.Empty;
}

public partial class CategoryCheck : ObservableObject
{
    public Category Category { get; }

    [ObservableProperty]
    private bool _isChecked;

    public CategoryCheck(Category category)
    {
        Category = category;
    }
}

public partial class RecipeEditViewModel : ObservableObject
{
    private readonly RecipeService _service = new();
    private readonly int _recipeId;
    private string? _chosenImagePath;
    private int _authorId;
    private DateTime _createdAt;
    private string? _oldImagePath;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _cookinTimeText = "30";

    [ObservableProperty]
    private string _servingsText = "2";

    [ObservableProperty]
    private Difficulty? _selectedDifficulty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _imageButtonText = "Выбрать картинку";

    public ObservableCollection<Difficulty> Difficulties { get; } = new();
    public ObservableCollection<Ingredient> Ingredients { get; } = new();
    public ObservableCollection<CategoryCheck> CategoryChecks { get; } = new();
    public ObservableCollection<IngredientRow> IngredientRows { get; } = new();
    public ObservableCollection<StepRow> StepRows { get; } = new();

    public event Action? Saved;

    public RecipeEditViewModel(int recipeId)
    {
        _recipeId = recipeId;
        LoadReferences();

        if (recipeId > 0)
            LoadRecipe();
        else
        {
            IngredientRows.Add(new IngredientRow());
            StepRows.Add(new StepRow());
        }
    }

    private void LoadReferences()
    {
        try
        {
            foreach (var d in _service.GetDifficulties()) Difficulties.Add(d);
            foreach (var i in _service.GetIngredients()) Ingredients.Add(i);
            foreach (var c in _service.GetCategories()) CategoryChecks.Add(new CategoryCheck(c));
            SelectedDifficulty = Difficulties.FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить справочники: " + ex.Message;
        }
    }

    private void LoadRecipe()
    {
        try
        {
            var recipe = _service.GetForEdit(_recipeId);
            if (recipe == null)
            {
                ErrorMessage = "Рецепт не найден";
                return;
            }

            _authorId = recipe.AuthorID;
            _createdAt = recipe.CreatedAt;
            _oldImagePath = recipe.ImagePath;

            Title = recipe.Title;
            Description = recipe.Description ?? string.Empty;
            CookinTimeText = recipe.CookinTime.ToString();
            ServingsText = recipe.Servings.ToString();
            SelectedDifficulty = Difficulties
                .FirstOrDefault(d => d.DifficultyID == recipe.DifficultyID);

            var checkedIds = recipe.RecipeCategories.Select(rc => rc.CategoryID).ToHashSet();
            foreach (var check in CategoryChecks)
                check.IsChecked = checkedIds.Contains(check.Category.CategoryID);

            foreach (var ri in recipe.RecipeIngredients)
                IngredientRows.Add(new IngredientRow
                {
                    SelectedIngredient = Ingredients
                        .FirstOrDefault(i => i.IngredientID == ri.IngredientID),
                    Amount = ri.Amount.ToString("0.##"),
                    Unit = ri.Unit ?? string.Empty
                });

            foreach (var step in recipe.Steps)
                StepRows.Add(new StepRow { Text = step.Text });

            if (_oldImagePath != null)
                ImageButtonText = "Картинка: " + _oldImagePath;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить рецепт: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AddIngredient() => IngredientRows.Add(new IngredientRow());

    [RelayCommand]
    private void RemoveIngredient(IngredientRow row) => IngredientRows.Remove(row);

    [RelayCommand]
    private void AddStep() => StepRows.Add(new StepRow());

    [RelayCommand]
    private void RemoveStep(StepRow row) => StepRows.Remove(row);

    [RelayCommand]
    private void ChooseImage()
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите картинку рецепта",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                _chosenImagePath = dialog.FileName;
                ImageButtonText = "Картинка: " + Path.GetFileName(dialog.FileName);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось открыть файл: " + ex.Message;
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Введите название рецепта";
            return;
        }
        if (!int.TryParse(CookinTimeText, out var time) || time <= 0)
        {
            ErrorMessage = "Время приготовления должно быть положительным числом";
            return;
        }
        if (!int.TryParse(ServingsText, out var servings) || servings <= 0)
        {
            ErrorMessage = "Количество порций должно быть положительным числом";
            return;
        }

        var steps = StepRows.Where(r => !string.IsNullOrWhiteSpace(r.Text)).ToList();
        if (steps.Count == 0)
        {
            ErrorMessage = "Добавьте хотя бы один шаг приготовления";
            return;
        }

        var ingredientData = new List<(IngredientRow row, decimal amount)>();
        foreach (var row in IngredientRows.Where(r => r.SelectedIngredient != null))
        {
            if (!decimal.TryParse(row.Amount.Replace('.', ','), out var amount) || amount <= 0)
            {
                ErrorMessage = $"Укажите количество для ингредиента «{row.SelectedIngredient!.IngredientName}»";
                return;
            }
            ingredientData.Add((row, amount));
        }
        if (ingredientData.Count == 0)
        {
            ErrorMessage = "Добавьте хотя бы один ингредиент";
            return;
        }

        string? imagePath = _oldImagePath;
        if (_chosenImagePath != null)
        {
            try
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");
                Directory.CreateDirectory(dir);
                var name = Guid.NewGuid().ToString() + Path.GetExtension(_chosenImagePath);
                File.Copy(_chosenImagePath, Path.Combine(dir, name), true);
                imagePath = name;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Не удалось сохранить картинку: " + ex.Message;
                return;
            }
        }

        var recipe = new Recipe
        {
            RecipeID = _recipeId,
            Title = Title.Trim(),
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
            CookinTime = time,
            Servings = servings,
            ImagePath = imagePath,
            DifficultyID = SelectedDifficulty?.DifficultyID ?? 1,
            AuthorID = _recipeId > 0 ? _authorId : Session.CurrentUser!.UserID,
            CreatedAt = _recipeId > 0 ? _createdAt : DateTime.Now
        };

        for (var i = 0; i < steps.Count; i++)
            recipe.Steps.Add(new RecipeStep { StepNumber = i + 1, Text = steps[i].Text.Trim() });

        for (var i = 0; i < ingredientData.Count; i++)
        {
            var (row, amount) = ingredientData[i];
            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientID = row.SelectedIngredient!.IngredientID,
                Amount = amount,
                Unit = string.IsNullOrWhiteSpace(row.Unit) ? null : row.Unit.Trim(),
                SortOrder = i + 1
            });
        }

        foreach (var check in CategoryChecks.Where(c => c.IsChecked))
            recipe.RecipeCategories.Add(new RecipeCategory { CategoryID = check.Category.CategoryID });

        try
        {
            _service.SaveRecipe(recipe);
            Saved?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось сохранить рецепт: " + ex.Message;
        }
    }
}