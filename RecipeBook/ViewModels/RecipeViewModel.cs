using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class RecipeViewModel : ObservableObject
{
    private readonly RecipeService _service = new();
    private readonly int _recipeId;

    [ObservableProperty]
    private Recipe? _recipe;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _commentText = string.Empty;

    [ObservableProperty]
    private bool _isCookedByMe;

    [ObservableProperty]
    private string _cookedButtonText = string.Empty;

    [ObservableProperty]
    private bool _canManage;

    public bool CanInteract => Session.IsLogged;
    public string AuthHint => Session.IsLogged
        ? string.Empty
        : "Войдите, чтобы оставлять комментарии и отмечать приготовленное";

    public RecipeViewModel(int recipeId)
    {
        _recipeId = recipeId;
        Load();
    }

    public void Load()
    {
        try
        {
            Recipe = _service.GetRecipe(_recipeId);
            CanManage = Session.IsLogged && Recipe != null &&
            (Session.IsAdmin || Session.CurrentUser!.UserID == Recipe.AuthorID);
            IsCookedByMe = Session.IsLogged
                && _service.IsCooked(_recipeId, Session.CurrentUser!.UserID);
            UpdateCookedButtonText();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить рецепт: " + ex.Message;
        }
    }

    [RelayCommand]
    private void ToggleCooked()
    {
        if (!Session.IsLogged)
        {
            ErrorMessage = "Для отметки нужно войти в систему";
            return;
        }

        try
        {
            _service.ToggleCooked(_recipeId, Session.CurrentUser!.UserID);
            IsCookedByMe = !IsCookedByMe;
            UpdateCookedButtonText();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось обновить отметку: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AddComment()
    {
        if (!Session.IsLogged)
        {
            ErrorMessage = "Для комментариев нужно войти в систему";
            return;
        }

        if (string.IsNullOrWhiteSpace(CommentText))
        {
            ErrorMessage = "Комментарий не может быть пустым";
            return;
        }

        try
        {
            _service.AddComment(_recipeId, Session.CurrentUser!.UserID, CommentText.Trim());
            CommentText = string.Empty;
            Load();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось добавить комментарий: " + ex.Message;
        }
    }

    private void UpdateCookedButtonText()
    {
        CookedButtonText = IsCookedByMe
            ? "Приготовлено — убрать отметку"
            : "Отметить как приготовленное";
    }

    public string? DeleteRecipe()
    {
        try
        {
            _service.DeleteRecipe(_recipeId);
            return null;
        }
        catch (Exception ex)
        {
            return "Не удалось удалить рецепт: " + ex.Message;
        }
    }
}