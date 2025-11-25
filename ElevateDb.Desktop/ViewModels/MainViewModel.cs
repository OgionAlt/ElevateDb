using CommunityToolkit.Mvvm.ComponentModel;

namespace ElevateDb.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private ElevateDbTestViewModel _elevateDbTestViewModel;

    public MainViewModel()
    {
        ElevateDbTestViewModel = new ElevateDbTestViewModel();
    }

    public ElevateDbTestViewModel ElevateDbTestViewModel
    {
        get => _elevateDbTestViewModel;
        set => SetProperty(ref _elevateDbTestViewModel, value);
    }
}