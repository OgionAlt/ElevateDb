using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElevateDb.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ElevateDb.Desktop.ViewModels;

public class ElevateDbDataViewModel : ObservableObject
{
    private readonly ApplicationContext _appContext;

    private string _selectedTable;
    private DataTable _tableData;

    public ElevateDbDataViewModel(ApplicationContext appContext)
    {
        _appContext = appContext;

        DbTableNames = new ObservableCollection<string>();
        ColumnMetadataInfos = new ObservableCollection<ColumnMetadataInfo>();

        InitializeCommands();
    }

    public ObservableCollection<string> DbTableNames { get; }
    public ObservableCollection<ColumnMetadataInfo> ColumnMetadataInfos { get; }

    public ICommand LoadDbTablesCommand { get; private set; }
    public ICommand LoadDbTableDataCommand { get; private set; }
    public ICommand ConfigTestCommand { get; private set; }
    public ICommand ElevateDbTestCommand { get; private set; }

    private void InitializeCommands()
    {
        LoadDbTablesCommand = new RelayCommand(LoadDbTables);
        LoadDbTableDataCommand = new RelayCommand(LoadDbTableData);
        ConfigTestCommand = new RelayCommand(ConfigTest);
        ElevateDbTestCommand = new RelayCommand(ElevateDbTest);
    }

    private void UpdateColumnMetadata()
    {
    }

    #region Properties

    public string SelectedTable
    {
        get => _selectedTable;
        set
        {
            if (SetProperty(ref _selectedTable, value)) LoadDbTableDataCommand.Execute(null);
        }
    }

    public DataTable TableData
    {
        get => _tableData;
        set => SetProperty(ref _tableData, value);
    }

    #endregion

    #region Command Handlers

    private void LoadDbTables()
    {
        try
        {
            using (var connection = new OdbcConnection(_appContext.DbConnectionString))
            {
                connection.Open();

                // Get schema information for tables
                var tables = connection.GetSchema("Tables");

                var tableNames = new List<string>();
                foreach (DataRow row in tables.Rows)
                {
                    // "TABLE_NAME" column contains the table name
                    var tableName = row["TABLE_NAME"].ToString();
                    tableNames.Add(tableName);
                    DbTableNames.Add(tableName);
                }


                // Example: Show table names in a message box (WPF)
                Debug.Print("Tables:\n" + string.Join("\n", tableNames), "ElevateDB Tables");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "ODBC Error");
        }
    }

    private void LoadDbTableData()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedTable))
                return;

            TableData = GetTableData(SelectedTable);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "ODBC Error");
        }
    }

    private DataTable GetTableData(string tableName)
    {
        var dataTable = new DataTable();
        using (var connection = new OdbcConnection(_appContext.DbConnectionString))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                // cmd.CommandText = $"SELECT TOP 100 * FROM {tableName}"; // Use LIMIT for MySQL/PostgreSQL
                cmd.CommandText = $"SELECT * FROM {tableName}"; // Use LIMIT for MySQL/PostgreSQL
                using (var adapter = new OdbcDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
        }

        ColumnMetadataInfos.Clear();

        using (var connection = new OdbcConnection(_appContext.DbConnectionString))
        {
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {SelectedTable}";
                using (var reader = cmd.ExecuteReader())
                {
                    // Get column names and data types
                    var columnInfo = new List<string>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var type = reader.GetDataTypeName(i);
                        // columnInfo.Add($"{name} ({type})");
                        ColumnMetadataInfos.Add(new ColumnMetadataInfo
                        {
                            ColumnName = name,
                            SqlColumnType = type
                        });
                    }
                }
            }
        }

        // foreach (DataColumn column in dataTable.Columns)
        //     ColumnMetadataInfos.Add(new ColumnMetadataInfo
        //     {
        //         ColumnName = column.ColumnName,
        //         ColumnType = column.DataType.ToString()
        //     });

        return dataTable;
    }

    private void ConfigTest()
    {
    }

    public void ElevateDbTest()
    {
        // Pseudocode:
        // 1. Create OdbcConnection using DSN "ElavateDBTutorial"
        // 2. Open connection
        // 3. Query all table names from the database (using ODBC metadata)
        // 4. Display or process the table names
        try
        {
            using (var connection = new OdbcConnection(_appContext.DbConnectionString))
                // using (var connection = new OdbcConnection(dsnConnectionString))
            {
                connection.Open();

                // Get schema information for tables
                var tables = connection.GetSchema("Tables");

                var tableNames = new List<string>();
                foreach (DataRow row in tables.Rows)
                    // "TABLE_NAME" column contains the table name
                    tableNames.Add(row["TABLE_NAME"].ToString());


                // Example: Show table names in a message box (WPF)
                MessageBox.Show("Tables:\n" + string.Join("\n", tableNames), "ElevateDB Tables");

                // Check if 'Customer' table exists
                if (tableNames.Contains("Customer"))
                {
                    // Query all records from 'Customer' table
                    var customers = new StringBuilder();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM Customer";
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Get column names and data types
                            var columnInfo = new List<string>();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var name = reader.GetName(i);
                                var type = reader.GetDataTypeName(i);
                                columnInfo.Add($"{name} ({type})");
                            }

                            customers.AppendLine(string.Join("\t", columnInfo));

                            // Get row data
                            while (reader.Read())
                            {
                                var rowValues = new List<string>();
                                for (var i = 0; i < reader.FieldCount; i++)
                                    rowValues.Add(reader[i]?.ToString() ?? string.Empty);
                                customers.AppendLine(string.Join("\t", rowValues));
                            }
                        }
                    }

                    // Display all records in a message box
                    MessageBox.Show(customers.ToString(), "Customer Table Records");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "ODBC Error");
        }
    }

    #endregion
}

public class ColumnMetadataInfo
{
    public string ColumnName { get; set; }
    public string SqlColumnType { get; set; }
    public string ColumnType { get; set; }
}