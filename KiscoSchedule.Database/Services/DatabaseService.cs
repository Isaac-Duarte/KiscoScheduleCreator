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
                'UserId'    INTEGER
	            'Name'	BLOB
            );";

            command = new SQLiteCommand(queryString, sqliteConnection);
            await command.ExecuteNonQueryAsync();

            // Create Role table
            queryString = @"CREATE TABLE IF NOT EXISTS 'Roles' (
	            'Id'	INTEGER PRIMARY KEY AUTOINCREMENT,
                'UserId'    INTEGER
	            'Name'	BLOB
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
                    UnableWeekDays = Employee.ConvertWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableWeekDays"])),
                    PerferedWorkingDays = Employee.ConvertWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["PerferedWorkingDays"])),
                    UnableSpecificDays = Employee.ConvertUnableSpecificDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableSpecificDays"]))
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
                employee.UnableWeekDays = Employee.ConvertWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableWeekDays"]));
                employee.PerferedWorkingDays = Employee.ConvertWeekDays(cryptoService.DecryptBytesToString((byte[])dataReader["PerferedWorkingDays"]));
                employee.UnableSpecificDays = Employee.ConvertUnableSpecificDays(cryptoService.DecryptBytesToString((byte[])dataReader["UnableSpecificDays"]));
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
            command.Parameters.AddWithValue("PerferedWorkingDays", cryptoService.EncryptString(Employee.ConvertWeekDays(employee.PerferedWorkingDays)));
            command.Parameters.AddWithValue("UnableWeekDays", cryptoService.EncryptString(Employee.ConvertWeekDays(employee.UnableWeekDays)));
            command.Parameters.AddWithValue("UnableSpecificDays", cryptoService.EncryptString(Employee.ConvertUnableSpecificDays(employee.UnableSpecificDays)));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Creates a shift async
        /// </summary>
        /// <param name="user">The parent of the shift</param>
        /// <param name="shift">The shift wanting to be created</param>
        /// <returns></returns>
        public async Task CreateShiftAsync(IUser user, IShift shift)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Shifts (UserId, Name) VALUES(@UserId, @Name)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptString(shift.Name));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Generates a list of shifts from the databse
        /// </summary>
        /// <param name="user">User wanting to be added to</param>
        /// <returns></returns>
        public async Task<List<IShift>> GetShiftsAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand(@"SELECT * FROM Shifts WHERE UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();
            List<IShift> shifts = new List<IShift>();

            while (dataReader.Read())
            {
                shifts.Add(new Shift
                {
                    Id = dataReader.GetInt32(0),
                    Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"])
                });
            }

            return shifts;
        }

        /// <summary>
        /// Creates a role async
        /// </summary>
        /// <param name="user">The parent of the role</param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task CreateRoleAsync(IUser user, IRole role)
        {
            SQLiteCommand command = new SQLiteCommand("INSERT INTO Roles (UserId, Name, Shifts) VALUES(@UserId, @Name, @Shifts)", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);
            command.Parameters.AddWithValue("Name", cryptoService.EncryptString(role.Name));
            command.Parameters.AddWithValue("Shifts", cryptoService.EncryptString(Role.ConvertShifts(role.Shifts)));

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Gets a list of roles
        /// </summary>
        /// <param name="user">Parent of user</param>
        /// <returns></returns>
        public async Task<List<IRole>> GetRolesAsync(IUser user)
        {
            SQLiteCommand command = new SQLiteCommand(@"SELECT * FROM Roles WHERE UserId=@UserId", sqliteConnection);
            command.Parameters.AddWithValue("UserId", user.Id);

            DbDataReader dataReader = await command.ExecuteReaderAsync();
            List<IRole> roles = new List<IRole>();

            while (dataReader.Read())
            {
                roles.Add(new Role
                {
                    Id = dataReader.GetInt32(0),
                    Name = cryptoService.DecryptBytesToString((byte[])dataReader["Name"]),
                    Shifts = Role.ConvertShifts(cryptoService.DecryptBytesToString((byte[])dataReader["Shifts"]))
                });
            }

            return roles;
        }
    }
}
