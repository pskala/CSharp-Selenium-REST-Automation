using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mySpaceName.Helpers.WebDriver
{
    public class SeleniumSettings
    {
        public string AppURL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Browser { get; set; }
        public bool RemoteDriver { get; set; }
        public string RemoteURL { get; set; }
        public string DownloadPathDir { get; set; }
        public string DataPathDir { get; set; }
        public TestContext TestContext { get; set; }
    }

}