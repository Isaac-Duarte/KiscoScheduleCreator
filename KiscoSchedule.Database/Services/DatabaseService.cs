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
    public class DatabaseService
    {
        private SQLiteConnection sqliteConnection;
        private CryptoService cryptoService;

        /// <summary>
        /// Initalizes the database service
        /// Sadly, the simple container doesn't allow for paramarter injection
        /// </summary>
        public DatabaseService()
        {
            cryptoService = new CryptoService();
        }

        /// <summary>
        /// Initalizer for SQLite Database
        /// </summary>
        /// <param name="folderName">The folder name of the desiered database location</param>
        /// <param name="databaseName">The sqlite database name</param>
        public DatabaseService(string folderName, string databaseName)
        {
            cryptoService = new CryptoService();
            FileUtil.CreateFolder(folderName);
            sqliteConnection = new SQLiteConnection($@"Data Source={folderName}\{databaseName};Version=3;");
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
	            'PhoneNumber'	BLOB,
	            'UnableWeekDays'	BLOB,
                'PerferedWorkingDays'	BLOB,
	            'UnableSpecificDays'	BLOB,
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

        /// <summary>
        /// Converts a list of weeks days to a string
        /// </summary>
        /// <param name="weekDays">List wanting to be converted</param>
        /// <returns></returns>
        private string weekDaysToString(List<DayOfWeek> weekDays)
        {
            return String.Join(",", weekDays.ToArray());
        }

        /// <summary>
        /// Converts a string to a list of week days
        /// </summary>
        /// <param name="weekDaysString">string wanting to be converted</param>
        /// <returns></returns>
        private List<DayOfWeek> stringToWeekDays(string weekDaysString)
        {
            List<DayOfWeek> weekDays = new List<DayOfWeek>();

            foreach (string weekDay in weekDaysString.Split(','))
            {
                weekDays.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), weekDay));
            }

            return weekDays;
        }

        /// <summary>
        /// Converts a list of date times to a string
        /// </summary>
        /// <param name="dateTimes"></param>
        /// <returns></returns>
        private string dateTimeToString(List<DateTime> dateTimes)
        {
            return String.Join(",", dateTimes.ToArray());
        }

        /// <summary>
        /// Converts a string to a list of date times
        /// </summary>
        /// <param name="dateTimesString">string wanting to be converted</param>
        /// <returns></returns>
        private List<DateTime> stringToDateTimes(string dateTimesString)
        {
            List<DateTime> dateTimes = new List<DateTime>();

            foreach (string dateTime in dateTimesString.Split(','))
            {
                dateTimes.Add(DateTime.Parse(dateTime));
            }

            return dateTimes;
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
                    PhoneNumber = cryptoService.DecryptBytesToString((byte[])dataReader["PhoneNumber"]),
                    UnableWeekDays = stringToWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableWeekDays"])),
                    PerferedWorkingDays = stringToWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["PerferedWorkingDays"])),
                    UnableSpecificDays = stringToDateTimes(cryptoService.DecryptBytesToString((byte[])dataReader["UnableSpecificDays"]))
                });
            }

            return employees;
        }

        /// <summary>
        /// Grabs an employee from the database
        /// </summary>
        /// <param name="id">The Id of the employee</param>
        /// <returns>List of employees</returns>
        public async Task<IEmployee> GetEmployee(int id)
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
                employee.UnableWeekDays = stringToWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableWeekDays"]));
                employee.PerferedWorkingDays = stringToWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["PerferedWorkingDays"]));
                employee.UnableSpecificDays = stringToDateTimes(cryptoService.DecryptBytesToString((byte[])dataReader["UnableSpecificDays"]));
            }

            return employee;
        }

        /// <summary>
        /// Adds an employee to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        public async Task CreateEmployeeAsync(IUser user, IEmployee employee)
        {
            List<string> dateTimes = new List<string>();

            foreach (DateTime dateTime in employee.UnableSpecificDays)
            {
                dateTimes.Add(dateTime.ToString("d", CultureInfo.CreateSpecificCulture("es-ES")));
            }

            SQLiteCommand command = new SQLiteCommand("INSERT INTO Employees  (UserId, Name, PhoneNumber, PerferedWorkingDays, UnableWeekDays, UnableSpecificDays) VALUES(@UserId, @Name, @PhoneNumber, @PerferedWorkingDays, @UnableWeekDays, @UnableSpecificDays)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.Name)));
            command.Parameters.AddWithValue("PhoneNumber", cryptoService.EncryptBytes(Encoding.UTF8.GetBytes(employee.PhoneNumber)));
            command.Parameters.AddWithValue("PerferedWorkingDays", cryptoService.EncryptString(weekDaysToString(employee.PerferedWorkingDays)));
            command.Parameters.AddWithValue("UnableWeekDays", cryptoService.EncryptString(weekDaysToString(employee.UnableWeekDays)));
            command.Parameters.AddWithValue("UnableSpecificDays", cryptoService.EncryptString(dateTimeToString(employee.UnableSpecificDays)));

            await command.ExecuteNonQueryAsync();
        }
    }
}
