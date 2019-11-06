/**
 * Common help procedures for manupulation with file
 **/
using Newtonsoft.Json;
using mySpaceName.Helpers.WebDriver;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mySpaceName.Helpers
{
    static class Extensions
    {
        public const string PLACEHOLDERVALUE = "$VALUE$";
        public const string FILETYPE = ".json";

        public static void SaveData(Data data, string name, SeleniumSettings settings)
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(@settings.DataPathDir + name + FILETYPE, json, Encoding.UTF8);
        }
        public static Data LoadData(string name, SeleniumSettings settings)
        {
            try
            {
                string json = File.ReadAllText(@settings.DataPathDir + name + FILETYPE);
                return JsonConvert.DeserializeObject<Data>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static void WaitForDownloadedFile(string filePath)
        {
            int stopLoop = 60;
            while (!File.Exists(filePath))
            {
                Thread.Sleep(1000);
                stopLoop--;
                if (stopLoop == 0)
                {
                    Assert.Fail("Cannot find downloaded file: " + filePath);
                }
            }
        }
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException)
                {
                    return;
                }
            }
        }
        public static void CopyFile(string from, string to)
        {
            DeleteFile(to);
            File.Copy(@from, @to);
        }
    }
}
