using System.Windows;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class RecipeEditWindow : Window
{
    public RecipeEditWindow(int recipeId)
    {
        InitializeComponent();
        var vm = new RecipeEditViewModel(recipeId);
        DataContext = vm;
        vm.Saved += () => Close();
        Title = recipeId > 0 ? "Редактирование рецепта" : "Новый рецепт";
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}