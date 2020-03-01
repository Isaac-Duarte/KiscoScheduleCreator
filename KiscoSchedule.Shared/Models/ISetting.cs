namespace KiscoSchedule.Shared.Models
{
    public interface ISetting
    {
        /// <summary>
        /// SQLite Id of the setting
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// The SQLite UserId
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// The key string of the setting
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        string Value { get; set; }
    }
}