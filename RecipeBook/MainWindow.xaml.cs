using RecipeBook.Models;
using RecipeBook.Services;
using RecipeBook.Views;
using System.Windows;
using System.Windows.Input;
using RecipeBook.ViewModels;

namespace RecipeBook;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AdminButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AdminWindow();
        window.Closed += (_, _) => ((MainViewModel)DataContext).LoadFilters();
        window.Show();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (Session.IsLogged)
        {
            new AuthService().Logout();
            MessageBox.Show("Вы вышли из аккаунта", "Книга рецептов",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            var authWindow = new AuthWindow();
            authWindow.ShowDialog();
        }
        ((MainViewModel)DataContext).RefreshSession();
    }

    private void RecipeCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is Recipe recipe)
        {
            var window = new RecipeWindow(recipe.RecipeID);
            window.Closed += (_, _) => ((MainViewModel)DataContext).LoadRecipes();
            window.Show();
        }
    }

    private void AddRecipe_Click(object sender, RoutedEventArgs e)
    {
        if (!Session.IsLogged)
        {
            MessageBox.Show("Добавлять рецепты могут только авторизованные пользователи",
                "Книга рецептов", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var window = new RecipeEditWindow(0);
        window.Closed += (_, _) => ((MainViewModel)DataContext).LoadRecipes();
        window.Show();
    }
    private void CookedButton_Click(object sender, RoutedEventArgs e)
    {
        if (!Session.IsLogged) return;
        var window = new CookedWindow(Session.CurrentUser!.UserID);
        window.Show();
    }
}