using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class CookedViewModel : ObservableObject
{
    private readonly RecipeService _service = new();
    private readonly int _userId;

    [ObservableProperty]
    private ObservableCollection<CookedRecord> _records = new();

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CookedViewModel(int userId)
    {
        _userId = userId;
        Load();
    }

    public void Load()
    {
        try
        {
            Records = new ObservableCollection<CookedRecord>(_service.GetCooked(_userId));
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Не удалось загрузить список: " + ex.Message;
        }
    }
}