﻿using KiscoSchedule.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KiscoSchedule.Database.Services
{
    public interface IDatabaseService
    {
        /// <summary>
        /// The path of the database
        /// </summary>
        string FolderPath { get; set; }

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
        /// Grabs a list of employees from the database
        /// </summary>
        /// <param name="userId">The parent id of the employee</param>
        /// <param name="limit">The limit (page)</param>
        /// <param name="offset">Offset</param>
        /// <returns>List of employees</returns>
        Task<List<IEmployee>> GetEmployeesAsync(IUser user);

        /// <summary>
        /// Adds an employee to the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        Task<long> CreateEmployeeAsync(IUser user, IEmployee employee);

        /// <summary>
        /// Updates an employee (had to rush temp will fix later)
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task UpdateEmployeeAsync(IEmployee employee);

        /// <summary>
        /// Deletes an employee from the database
        /// </summary>
        /// <param name="user"></param>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task DeleteEmployeeAsync(IEmployee employee);

        /// <summary>
        /// Creates a Setting given the setting and employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task<long> CreateSettingAsync(IUser user, ISetting setting);

        /// <summary>
        /// Updates a setting given the setting object
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task UpdateSettingAsync(ISetting setting);

        /// <summary>
        /// Returns a list of the settings
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task<Dictionary<string, ISetting>> GetSettingsAsync(IUser user);

        /// <summary>
        /// Removes a setting given the setting object
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task RemoveSettingAsync(ISetting setting);

        /// <summary>
        /// Creates a shift async
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        Task<long> CreateShiftAsync(IUser user, IShift shift);

        /// <summary>
        /// Updates a shift given the shift object
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task UpdateShiftAsync(IShift shift);

        /// <summary>
        /// Returns a list of the settings
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        Task<List<IShift>> GetShiftsAsync(IUser user);

        /// <summary>
        /// Removes a shift given the shift object
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        Task RemoveShiftsAsync(IShift shift);
    }
}