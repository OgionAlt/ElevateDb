using System;
using System.IO;

namespace ElevateDb.Desktop.Utils;

public static class ElevateDbUtils
{
    public static string CreateElevateDbConnectionString(string user, string password, string dbPath,
        string dbName = null, bool convertToAbsolutePath = false)
    {
        // var connectionString = @"DRIVER={{ElevateDB 2 ODBC Driver}};
        // CHARSET=UNICODE;
        // TYPE=LOCAL;
        // UID=Administrator;
        // PWD=EDBDefault;
        // CONFIGPATH=C:\Tutorial;
        // DATABASE=Tutorial";

        var connectionString = $@"DRIVER={{ElevateDB 2 ODBC Driver}};
CHARSET=UNICODE;
TYPE=LOCAL;
UID={user};
PWD={password}";

        var dbPathResult = dbPath;
        if (convertToAbsolutePath && Directory.Exists(dbPath))
            dbPathResult = new DirectoryInfo(dbPath).FullName;
        connectionString += $";{Environment.NewLine}CONFIGPATH={dbPathResult}";

        if (!string.IsNullOrWhiteSpace(dbName)) connectionString += $";{Environment.NewLine}DATABASE=Tutorial";

        return connectionString;
    }

    public static string CreateElevateDbConnectionString(string dsn)
    {
        // var dsnConnectionString = "DSN=ElavateDBTutorial";
        var dsnConnectionString = $"DSN={dsn}";
        return dsnConnectionString;
    }
}