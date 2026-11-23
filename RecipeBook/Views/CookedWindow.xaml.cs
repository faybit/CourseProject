using System.Windows;
using RecipeBook.Models;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class CookedWindow : Window
{
    public CookedWindow(int userId)
    {
        InitializeComponent();
        DataContext = new CookedViewModel(userId);
    }

    private void OpenRecipe_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is CookedRecord record)
        {
            var window = new RecipeWindow(record.RecipeID);
            window.Closed += (_, _) => ((CookedViewModel)DataContext).Load();
            window.Show();
        }
    }
}