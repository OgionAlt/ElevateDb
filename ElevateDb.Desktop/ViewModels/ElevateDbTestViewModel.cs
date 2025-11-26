using System;
using System.Data;
using System.Data.Odbc;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElevateDb.Desktop.Models;
using ElevateDb.Desktop.Settings;
using ElevateDb.Desktop.Utils;
using Microsoft.Extensions.Options;

namespace ElevateDb.Desktop.ViewModels;

public class ElevateDbTestViewModel : ObservableObject
{
    private readonly ApplicationContext _appContext;
    private readonly AppSettings _settings;
    private string _dbName;
    private string _dbPassword;
    private string _dbPath;
    private string _dbUser;

    private string _odbcDsn;

    public ElevateDbTestViewModel(IOptions<AppSettings> settings, ApplicationContext appContext)
    {
        _settings = settings.Value;
        _appContext = appContext;

        DbUser = _settings.ElevateDbUser;
        DbPassword = _settings.ElevateDbPassword;
        DbPath = _settings.DbPath;
        DbName = _settings.DbName;
        OdbcDsn = _settings.OdbcDsn;

        InitializeCommands();
    }

    public ICommand ApplyAndTestDbConnectionCommand { get; private set; }
    public ICommand ApplyAndTestDbDsnConnectionCommand { get; private set; }

    private void InitializeCommands()
    {
        ApplyAndTestDbConnectionCommand = new RelayCommand(ApplyAndTestDbConnection);
        ApplyAndTestDbDsnConnectionCommand = new RelayCommand(ApplyAndTestDbDsnConnection);
    }

    #region Command Handlers

    private void ApplyAndTestDbConnection()
    {
        CurrentDbConnectionString = ElevateDbUtils.CreateElevateDbConnectionString(DbUser, DbPassword, DbPath, DbName);
        OnPropertyChanged(nameof(CurrentDbConnectionString));

        TestDbConnection();
    }

    private void ApplyAndTestDbDsnConnection()
    {
        CurrentDbConnectionString = ElevateDbUtils.CreateElevateDbConnectionString(OdbcDsn);
        OnPropertyChanged(nameof(CurrentDbConnectionString));

        TestDbConnection();
    }

    public void TestDbConnection()
    {
        var isConnectionSuccessful = false;
        try
        {
            using (var connection = new OdbcConnection(CurrentDbConnectionString))
            {
                connection.Open();
                isConnectionSuccessful = connection.State == ConnectionState.Open;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        if (isConnectionSuccessful)
        {
            MessageBox.Show("Successfully connected to DB!");
            _appContext.DbConnectionString = CurrentDbConnectionString;
        }
    }

    #endregion

    #region Properties

    public string DbUser
    {
        get => _dbUser;
        set => SetProperty(ref _dbUser, value);
    }

    public string DbPassword
    {
        get => _dbPassword;
        set => SetProperty(ref _dbPassword, value);
    }

    public string DbPath
    {
        get => _dbPath;
        set => SetProperty(ref _dbPath, value);
    }

    public string DbName
    {
        get => _dbName;
        set => SetProperty(ref _dbName, value);
    }

    public string OdbcDsn
    {
        get => _odbcDsn;
        set => SetProperty(ref _odbcDsn, value);
    }

    public string CurrentDbConnectionString { get; private set; }

    #endregion
}