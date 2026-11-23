using System.Windows;
using RecipeBook.Data;

namespace RecipeBook;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            DbInitializer.Initialize();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось подключиться к базе данных:\n" + ex.Message,
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}