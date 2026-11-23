using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly AuthService _auth = new();

    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isRegisterMode;

    public string SubmitText => IsRegisterMode ? "Зарегистрироваться" : "Войти";
    public string SwitchText => IsRegisterMode ? "Уже есть аккаунт? Войти" : "Нет аккаунта? Зарегистрироваться";
    public string ModeTitle => IsRegisterMode ? "Регистрация" : "Вход в систему";

    partial void OnIsRegisterModeChanged(bool value)
    {
        OnPropertyChanged(nameof(SubmitText));
        OnPropertyChanged(nameof(SwitchText));
        OnPropertyChanged(nameof(ModeTitle));
    }

    [RelayCommand]
    private void SwitchMode()
    {
        IsRegisterMode = !IsRegisterMode;
        ErrorMessage = string.Empty;
        Password = string.Empty;
    }

    [RelayCommand]
    private void Submit()
    {
        var error = IsRegisterMode
            ? _auth.Register(Login, Password, DisplayName)
            : _auth.Login(Login, Password);

        if (error != null)
        {
            ErrorMessage = error;
            return;
        }

        SubmitSucceeded?.Invoke();
    }

    public event Action? SubmitSucceeded;
}