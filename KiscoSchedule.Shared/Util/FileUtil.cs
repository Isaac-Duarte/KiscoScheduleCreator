using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiscoSchedule.Shared.Util
{
    public class FileUtil
    {
        /// <summary>
        /// This grabs the %appdata% folder.
        /// </summary>
        /// <returns>The location of the appdata folder</returns>
        public static string GetAppDataFolder()
        {
            // Return the application data folder
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        
        /// <summary>
        /// Creates a folder if it doesn't exist
        /// </summary>
        /// <param name="path">Path of the folder</param>
        public static void CreateFolder(string path)
        {
            // Check to see if the directory doesn't exist 
            if (!Directory.Exists(path))
            {
                // Create the directory
                Directory.CreateDirectory(path);
            }
        }
    }
}
