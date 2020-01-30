using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiscoSchedule.Shared.Util;

namespace KiscoSchedule.Database.Services
{
    public class DatabaseService
    {
        private SQLiteConnection sqliteConnection;

        /// <summary>
        /// Initalizer for SQLite Database
        /// </summary>
        public DatabaseService(string folderName, string databaseName)
        {
            FileUtil.CreateFolder(folderName);
            sqliteConnection = new SQLiteConnection($@"Data Source={folderName}\{databaseName};Version=3;");
        }

        /// <summary>
        /// Opens the database Asn
        /// </summary>
        public async Task OpenAsync()
        {
            await sqliteConnection.OpenAsync();
            await createTables();
        }

        /// <summary>
        /// Creates the tables
        /// </summary>
        /// <returns></returns>
        private async Task createTables()
        {
            // Create user table
            string queryString = @"CREATE TABLE IF NOT EXISTS 'User' (
                'Id'        INTEGER PRIMARY KEY AUTOINCREMENT,
                'Username'  TEXT NOT NULL UNIQUE,
                'Password'  TEXT NOT NULL
             )";

            SQLiteCommand command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Employees table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Employees' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	            'UserId'	INTEGER,
	            'Name'	BLOB,
	            'PhoneNumber'	BLOB,
	            'UnableWeekDays'	BLOB,
	            'UnableWorkDays'	INTEGER,
	            'Roles'	BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Shift table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Shifts' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	            'Name'	BLOB,
	            'Roles'	BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Role table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Roles' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	            'Name'	BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Schedules table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Schedules' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	            'Date'	BLOB,
	            'Data'	BLOB
            );";
            
            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
