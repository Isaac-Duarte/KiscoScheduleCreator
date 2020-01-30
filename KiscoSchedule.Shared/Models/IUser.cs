namespace KiscoSchedule.Shared.Models
{
    public interface IUser
    {
        /// <summary>
        /// This SQLite Id of the user
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// The inital of a username
        /// </summary>
        char Inital { get; set; }

        /// <summary>
        /// Hash of the user's password
        /// </summary>
        string Hash { get; set; }
    }
}