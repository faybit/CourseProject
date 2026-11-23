using System.Windows;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class RecipeWindow : Window
{
    public RecipeWindow(int recipeId)
    {
        InitializeComponent();
        var vm = new RecipeViewModel(recipeId);
        DataContext = vm;
        Title = vm.Recipe?.Title ?? "Рецепт";
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (RecipeViewModel)DataContext;
        if (vm.Recipe == null) return;
        var window = new RecipeEditWindow(vm.Recipe.RecipeID);
        window.Closed += (_, _) => vm.Load();
        window.Show();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Удалить рецепт вместе с шагами, составом и комментариями?",
            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        var vm = (RecipeViewModel)DataContext;
        var error = vm.DeleteRecipe();
        if (error != null)
        {
            MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        Close();
    }
}