using KiscoSchedule.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiscoSchedule.Database.Services
{
    public interface IDatabaseService
    {
        /// <summary>
        /// Creates a SQLite connection
        /// </summary>
        /// <param name="folderName">The folder name of the desiered database location</param>
        /// <param name="databaseName">The sqlite database name</param>
        void CreateConnection(string folderName, string databaseName);

        /// <summary>
        /// Opens the database Asn
        /// </summary>
        Task OpenAsync();

        /// <summary>
        /// Set the passwor used for encrpytion
        /// </summary>
        /// <param name="password"></param>
        void SetPassword(string password);

        /// <summary>
        /// Grabs a user async by username
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>User</returns>
        Task<User> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Adds a user into the databse
        /// </summary>
        /// <param name="username">username of user</param>
        /// <param name="hash">password hash of user</param>
        /// </summary>
        Task CreateUserAsync(IUser user);

        /// <summary>
        /// Grabs a list of employees from the database
        /// </summary>
        /// <param name="userId">The parent id of the employee</param>
        /// <param name="limit">The limit (page)</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of employees</returns>
        Task<List<IEmployee>> GetEmployeesAsync(IUser user, int limit, int offset);

        /// <summary>
        /// Grabs an employee from the database
        /// </summary>
        /// <param name="id">The Id of the employee</param>
        /// <returns>List of employees</returns>
        Task<IEmployee> GetEmployeeAsync(int id);

        /// <summary>
        /// Adds an employee to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        Task CreateEmployeeAsync(IUser user, IEmployee employee);

        /// <summary>
        /// Creates a shift async
        /// </summary>
        /// <param name="user">The parent of the shift</param>
        /// <param name="shift">The shift wanting to be created</param>
        /// <returns></returns>
        Task CreateShiftAsync(IUser user, IShift shift);

        /// <summary>
        /// Generates a list of shifts from the databse
        /// </summary>
        /// <param name="user">User wanting to be added to</param>
        /// <returns></returns>
        Task<List<IShift>> GetShiftsAsync(IUser user);

        /// <summary>
        /// Creates a role async
        /// </summary>
        /// <param name="user">The parent of the role</param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task CreateRoleAsync(IUser user, IRole role);

        /// <summary>
        /// Gets a list of roles
        /// </summary>
        /// <param name="user">Parent of user</param>
        /// <returns></returns>
        Task<List<IRole>> GetRolesAsync(IUser user);
    }
}