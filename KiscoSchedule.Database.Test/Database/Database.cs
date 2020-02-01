using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace KiscoSchedule.Database.Test.Database
{
    [TestClass]
    class Database
    {
        [TestMethod]
        public async void CreateDatabaseInAppDataFolder()
        {
            DatabaseService database = new DatabaseService();

            string folder = FileUtil.GetAppDataFolder();

            database.CreateConnection(folder, "_.db");
            await database.OpenAsync();

            Assert.IsTrue(File.Exists($@"{folder}\_.db"));
        }
    }
}
