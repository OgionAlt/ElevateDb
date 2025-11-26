using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace ElevateDb.Desktop.ViewModels;

public class MainViewModel : ObservableObject
{
    private ElevateDbDataViewModel _elevateDbDataViewModel;
    private ElevateDbTestViewModel _elevateDbTestViewModel;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        ElevateDbTestViewModel = serviceProvider.GetRequiredService<ElevateDbTestViewModel>();
        ElevateDbDataViewModel = serviceProvider.GetRequiredService<ElevateDbDataViewModel>();
    }

    public ElevateDbTestViewModel ElevateDbTestViewModel
    {
        get => _elevateDbTestViewModel;
        set => SetProperty(ref _elevateDbTestViewModel, value);
    }

    public ElevateDbDataViewModel ElevateDbDataViewModel
    {
        get => _elevateDbDataViewModel;
        set => SetProperty(ref _elevateDbDataViewModel, value);
    }
}