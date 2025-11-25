using System.Windows;
using ElevateDb.Desktop.ViewModels;

namespace ElevateDb.Desktop;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var mainViewModel = new MainViewModel();
        var mainWindow = new MainWindow { DataContext = mainViewModel };
        mainWindow.Show();
    }
}