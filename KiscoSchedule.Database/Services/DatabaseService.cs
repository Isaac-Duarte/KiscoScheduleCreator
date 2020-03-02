using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KiscoSchedule.Shared.Models;
using KiscoSchedule.Shared.Util;

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
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
                'UserId'    INTEGER
	            'Date'	BLOB,
	            'Data'	BLOB
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

            // Create Shifts table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Shifts' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
                'UserId'    INTEGER,
	            'Name'	BLOB
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
            List<string> dateTimes = new List<string>();

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
        public async Task CreateSettingAsync(IUser user, ISetting setting)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Settings (UserId, Key, Value) VALUES(@UserId, @Key, @Value)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Key", cryptoService.EncryptString(setting.Key));
            command.Parameters.AddWithValue("Value", cryptoService.EncryptString(setting.Value));

            await command.ExecuteNonQueryAsync();
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
            command.Parameters.AddWithValue("Key", cryptoService.EncryptString(setting.Key));
            command.Parameters.AddWithValue("Value", cryptoService.EncryptString(setting.Value));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Returns a list of the settings
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, ISetting>> GetSettingsAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Settings Where UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            Dictionary<string, ISetting> settings = new Dictionary<string, ISetting>();
            
            while (dataReader.Read())
            {
                ISetting setting = new Setting
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Key = cryptoService.DecryptBytesToString((byte[])dataReader["Key"]),
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
        /// Creates a shift async
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public async Task CreateShiftAsync(IUser user, IShift shift)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Shifts (UserId, Name) VALUES(@UserId, @Name)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptString(shift.Name));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Updates a shift given the shift object
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public async Task UpdateShiftAsync(IShift shift)
        {
            SQLiteCommand command = new SQLiteCommand("Update Shifts Set Name = @Name WHERE Id = @Id", sqliteConnection);
            command.Parameters.AddWithValue("Id", shift.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptString(shift.Name));

            await command.ExecuteNonQueryAsync();
        }

         /// <summary>
        /// Returns a list of the settings
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public async Task<List<IShift>> GetShiftsAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Shifts Where UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();

            List<IShift> shifts = new List<IShift>();
            
            while (dataReader.Read())
            {
                shifts.Add(new Shift
                {
                    Id = dataReader.GetInt32(0),
                    UserId = dataReader.GetInt32(1),
                    Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"])
                });
            }

            return shifts;
        }
    }
}
