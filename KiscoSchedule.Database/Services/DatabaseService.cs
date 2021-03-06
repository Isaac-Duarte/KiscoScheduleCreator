﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiscoSchedule.Shared.Enums;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Shared.Util;
using Newtonsoft.Json;

namespace KiscoSchedule.Database.Services
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteConnection sqliteConnection;
        private CryptoService cryptoService;
        public string FolderPath { get; set; }

        /// <summary>
        /// Initalizes the database service
        /// Sadly, the simple container doesn't allow for paramarter injection
        /// </summary>
        public DatabaseService()
        {
            cryptoService = new CryptoService();
        }

        /// <summary>
        /// Creates the tables
        /// </summary>
        /// <returns></returns>
        private async Task createTables()
        {
            // Create user table
            string queryString = @"CREATE TABLE IF NOT EXISTS 'Users' (
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
	            'PhoneNumber'	BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Schedules table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Schedules' (
                'Id'    INTEGER PRIMARY KEY AUTOINCREMENT,
                'UserId'    INTEGER,
	            'Data'  BLOB,
	            'Date'  TEXT
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Settings table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Settings' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
                'UserId'    INTEGER,
	            'Key'	BLOB,
                'Value' BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

        }

        /// <summary>
        /// Creates a SQLite connection
        /// </summary>
        /// <param name="folderName">The folder name of the desiered database location</param>
        /// <param name="databaseName">The sqlite database name</param>
        public void CreateConnection(string folderName, string databaseName)
        {
            FileUtil.CreateFolder(folderName);
            sqliteConnection = new SQLiteConnection($@"Data Source={folderName}\{databaseName};Version=3;");
            FolderPath = $@"{folderName}\{databaseName}";
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
        /// Set the passwor used for encrpytion
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password)
        {
            cryptoService.GenerateCryptoProvider(password);
        }

        /// <summary>
        /// Grabs a user async by username
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>User</returns>
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            SQLiteCommand command = new SQLiteCommand(@"SELECT * FROM Users WHERE Username=@Username", sqliteConnection);
            command.Parameters.AddWithValue("Username", username.ToLower());

            DbDataReader dataReader = await command.ExecuteReaderAsync();
            User user = new User();

            while (dataReader.Read())
            {
                user.Id = dataReader.GetInt32(0);
                user.UserName = dataReader.GetString(1);
                user.Hash = dataReader.GetString(2);
            }

            return user;
        }

        /// <summary>
        /// Adds a user into the databse
        /// </summary>
        /// <param name="username">username of user</param>
        /// <param name="hash">password hash of user</param>
        /// </summary>
        public async Task CreateUserAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Users (Username, Password) VALUES(@Username, @Password)", sqliteConnection);
            command.Parameters.AddWithValue("Username", user.UserName.ToLower());
            command.Parameters.AddWithValue("Password", user.Hash);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Grabs a list of employees from the database
        /// </summary>
        /// <param name="userId">The parent id of the employee</param>
        /// <param name="limit">The limit (page)</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of employees</returns>
        public async Task<List<IEmployee>> GetEmployeesAsync(IUser user, int limit, int offset)
        {
            List<IEmployee> employees = new List<IEmployee>();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Employees Where UserId=@UserId LIMIT @Limit OFFSET @Offset", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Limit", limit);
            command.Parameters.AddWithValue("Offset", offset);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (dataReader.Read())
            {
                employees.Add(new Employee
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"]),
                    PhoneNumber = cryptoService.DecryptBytesToString((byte[])dataReader["PhoneNumber"])
                });
            }

            return employees;
        }

        /// <summary>
        /// Grabs a list of employees from the database
        /// </summary>
        /// <param name="userId">The parent id of the employee</param>
        /// <param name="limit">The limit (page)</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of employees</returns>
        public async Task<List<IEmployee>> GetEmployeesAsync(IUser user)
        {
            List<IEmployee> employees = new List<IEmployee>();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Employees Where UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (dataReader.Read())
            {
                employees.Add(new Employee
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"]),
                    PhoneNumber = cryptoService.DecryptBytesToString((byte[])dataReader["PhoneNumber"])
                });
            }

            return employees;
        }

        /// <summary>
        /// Grabs an employee from the database
        /// </summary>
        /// <param name="id">The Id of the employee</param>
        /// <returns>List of employees</returns>
        public async Task<IEmployee> GetEmployeeAsync(int id)
        {
            Employee employee = new Employee();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Employees Where Id=@Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            while (dataReader.Read())
            {
                employee.Id = dataReader.GetInt32(0);
                employee.UserId = dataReader.GetInt32(1);
                employee.Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"]);
                employee.PhoneNumber = cryptoService.DecryptBytesToString((byte[])dataReader["PhoneNumber"]);
            }

            return employee;
        }

        /// <summary>
        /// Adds an employee to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        public async Task<long> CreateEmployeeAsync(IUser user, IEmployee employee)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Employees (UserId, Name, PhoneNumber) VALUES(@UserId, @Name, @PhoneNumber)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.Name)));
            command.Parameters.AddWithValue("PhoneNumber", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.PhoneNumber)));

            await command.ExecuteNonQueryAsync();
            return sqliteConnection.LastInsertRowId;
        }

        /// <summary>
        /// Updates an employee (had to rush temp will fix later)
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task UpdateEmployeeAsync(IEmployee employee)
        {
            SQLiteCommand command = new SQLiteCommand("UPDATE Employees SET Name = @Name, PhoneNumber = @PhoneNumber WHERE Id = @Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", employee.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.Name)));
            command.Parameters.AddWithValue("PhoneNumber", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.PhoneNumber)));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes an employee from the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task DeleteEmployeeAsync(IEmployee employee)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Employees WHERE Id=@Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", employee.Id);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates a Setting given the setting and employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task<long> CreateSettingAsync(IUser user, ISetting setting)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Settings (UserId, Key, Value) VALUES(@UserId, @Key, @Value)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Key", cryptoService.EncryptBytes(BitConverter.GetBytes((int)setting.Key)));
            command.Parameters.AddWithValue("Value", cryptoService.EncryptString(setting.Value));

            await command.ExecuteNonQueryAsync();
            return sqliteConnection.LastInsertRowId;
        }

        /// <summary>
        /// Updates a setting given the setting object
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task UpdateSettingAsync(ISetting setting)
        {
            SQLiteCommand command = new SQLiteCommand("Update Settings Set Key = @Key, Value = @Value WHERE Id = @Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", setting.Id);
            command.Parameters.AddWithValue("Key", cryptoService.EncryptBytes(BitConverter.GetBytes((int)setting.Key)));
            command.Parameters.AddWithValue("Value", cryptoService.EncryptString(setting.Value));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Returns a list of the settings
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task<Dictionary<SettingEnum, ISetting>> GetSettingsAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Settings Where UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            Dictionary<SettingEnum, ISetting> settings = new Dictionary<SettingEnum, ISetting>();
            
            while (dataReader.Read())
            {
                ISetting setting = new Setting
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Key = (SettingEnum)BitConverter.ToInt32(cryptoService.DecryptBytes((byte[])dataReader["Key"]), 0),
                    Value = cryptoService.DecryptBytesToString((byte[])dataReader["Value"])
                };

                settings.Add(setting.Key, setting);
            }

            return settings;
        }

        /// <summary>
        /// Removes a setting given the setting object
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task RemoveSettingAsync(ISetting setting)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Settings Where Id=@Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", setting.Id);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates a Schedule async
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public async Task<long> CreateScheduleAsync(IUser user, ISchedule Schedule)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Schedules (UserId, Date, Data) VALUES(@UserId, @Date, @Data)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Date", Schedule.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("Data", cryptoService.EncryptString(JsonConvert.SerializeObject(Schedule.Shifts)));

            await command.ExecuteNonQueryAsync();
            return sqliteConnection.LastInsertRowId;
        }

        /// <summary>
        /// Updates a tempalte async
        /// </summary>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public async Task UpdateScheduleAsync(ISchedule Schedule)
        {
            SQLiteCommand command = new SQLiteCommand("Update Schedules Set Data = @Data WHERE Id = @Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", Schedule.Id);
            command.Parameters.AddWithValue("Data", cryptoService.EncryptString(JsonConvert.SerializeObject(Schedule.Shifts)));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Gets a Schedule async
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<List<ISchedule>> GetSchedulesAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Schedules Where UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            List<ISchedule> schedules = new List<ISchedule>();

            while (dataReader.Read())
            {
                schedules.Add(new Schedule
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Date = DateTime.Parse(cryptoService.DecryptBytesToString((byte[])dataReader["Date"])),
                    Shifts = JsonConvert.DeserializeObject<Dictionary<int, ShiftTemplate>>(cryptoService.DecryptBytesToString((byte[])dataReader["Data"]))
                });
            }

            return schedules;
        }

        /// <summary>
        /// Get a specific Schedule
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<ISchedule> GetScheduleAsync(DateTime date)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Schedules Where Date=@Date", sqliteConnection);
            command.Parameters.AddWithValue("Date", date.ToString("yyyy-MM-dd"));

            DbDataReader dataReader = await command.ExecuteReaderAsync();
            
            ISchedule schedule = new Schedule();

            while (dataReader.Read())
            {
                schedule.Id = dataReader.GetInt32(0);
                schedule.UserId = dataReader.GetInt32(1);
                schedule.Date = DateTime.Parse((string)dataReader["Date"]);
                schedule.Shifts = JsonConvert.DeserializeObject<Dictionary<int, ShiftTemplate>>(cryptoService.DecryptBytesToString((byte[])dataReader["Data"]));
            }

            return schedule;
        }

        /// <summary>
        /// Removes a tempalte async
        /// </summary>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public async Task RemoveScheduleAsync(ISchedule Schedule)
        {
            SQLiteCommand command = new SQLiteCommand("DELETE FROM Schedules Where Id=@Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", Schedule.Id);

            await command.ExecuteNonQueryAsync();
        }
    }
}
