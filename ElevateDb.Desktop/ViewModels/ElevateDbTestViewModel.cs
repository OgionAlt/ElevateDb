using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ElevateDb.Desktop.ViewModels
{
    public class ElevateDbTestViewModel: ObservableObject
    {
        public ICommand ElevateDbTestCommand { get; }

        public ElevateDbTestViewModel()
        {
            ElevateDbTestCommand = new RelayCommand(ElevateDbTest);
        }

        public void ElevateDbTest()
        {
            var connectionString = @"DRIVER={ElevateDB 2 ODBC Driver};
CHARSET=UNICODE;
TYPE=LOCAL;
UID=Administrator;
PWD=EDBDefault;
CONFIGPATH=C:\Tutorial;
DATABASE=Tutorial";

            var dsnConnectionString = "DSN=ElavateDBTutorial";

            // Pseudocode:
            // 1. Create OdbcConnection using DSN "ElavateDBTutorial"
            // 2. Open connection
            // 3. Query all table names from the database (using ODBC metadata)
            // 4. Display or process the table names
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                // using (var connection = new OdbcConnection(dsnConnectionString))
                {
                    connection.Open();

                    // Get schema information for tables
                    var tables = connection.GetSchema("Tables");

                    var tableNames = new List<string>();
                    foreach (System.Data.DataRow row in tables.Rows)
                    {
                        // "TABLE_NAME" column contains the table name
                        tableNames.Add(row["TABLE_NAME"].ToString());
                    }



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
                                // Get column names
                                var columnNames = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    columnNames.Add(reader.GetName(i));
                                }
                                customers.AppendLine(string.Join("\t", columnNames));

                                // Get row data
                                while (reader.Read())
                                {
                                    var rowValues = new List<string>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        rowValues.Add(reader[i]?.ToString() ?? string.Empty);
                                    }
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
    }
}
