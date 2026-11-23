using System.Windows;
using System.Windows.Controls;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class AuthWindow : Window
{
    public AuthWindow()
    {
        InitializeComponent();
        var vm = (AuthViewModel)DataContext;
        vm.SubmitSucceeded += () =>
        {
            DialogResult = true;
            Close();
        };
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
            vm.Password = PasswordBox.Password;
    }
}