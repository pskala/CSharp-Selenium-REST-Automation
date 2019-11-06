using OpenQA.Selenium;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using mySpaceName.Helpers.Api;
using System.IO;
using mySpaceName.Pages;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace mySpaceName.Helpers.WebDriver
{
    public class BaseTest
    {
        protected Context context;
        protected WebApi webapi;
        protected SeleniumSettings settings;
        protected IWebDriver driver;
        private const string seleniumTest = "Selenium";
        public bool prepareSeleniumEnv = true;
        private TestContext testContext;
        private string dateToday = "_date_" + DateTime.Now.ToString("yyyy-MM-dd") + "_time_" + DateTime.Now.ToString("HH-mm-ss");
        List<string> performanceOutputList = new List<string>();
        string performanceFilePath;
        DateTime startTest;

        [TestInitialize()]
        public virtual void TestInitialize()
        {
            LoadSettings();
            webapi = new WebApi(settings, performanceOutputList);
            try
            {
                AddTestInitialize();
            }
            catch (Exception e)
            {
                AddTestCleanup();
                throw e;
            }
            if (TestContext.TestName.Contains(seleniumTest) && prepareSeleniumEnv) // only for UI testcases, not for WebApi
            {
                CreateDriver();
                CreateContext();
                try
                {
                    OpenBrowser();
                    Login();
                    AddTestInitializeAfterSeleniumLogin();
                }
                catch (Exception e)
                {
                    driver.Quit();
                    driver.Dispose();
                    throw e;
                }
            }
            startTest = DateTime.Now;
        }
        public virtual void AddTestInitialize()
        {
        }
        public virtual void AddTestInitializeAfterSeleniumLogin()
        {
        }
        protected void CreateContext()
        {
            context = new Context(driver, settings.AppURL);
        }
        protected void Login(string userName = null, string password = null)
        {
            var loginPage = new LoginPage(context);
            if (userName == null) userName = settings.UserName;
            if (password == null) password = settings.Password;
            loginPage.Login(userName, password);
        }
        protected void OpenBrowser()
        {
            OpenBrowserInternal(context);
        }
        protected void CreateDriver()
        {
            if (settings.RemoteDriver)
            {
                driver = GetRemoteDriver(settings.RemoteURL);
            } else
            {
                driver = GetDriverByBrowser(settings.Browser);
            }
        }
        private static void OpenBrowserInternal(Context context)
        {
            context.SetURL();
            context.GetDriver().Manage().Window.Maximize();
        }
        protected void LoadSettings()
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            settings = new SeleniumSettings
            {
                AppURL = (string)testContext.Properties["webAppUrl"],
                UserName = (string)testContext.Properties["webAppUserName"],
                Password = (string)testContext.Properties["webAppPassword"],
                RemoteDriver = bool.Parse((string)testContext.Properties["remoteDriver"]),
                RemoteURL = (string)testContext.Properties["remoteUrl"],
                Browser = (string)testContext.Properties["Browser"],
                DownloadPathDir = TestContext.TestResultsDirectory + "\\",
                DataPathDir = path + "\\" + testContext.Properties["DataPathDir"],
                TestContext = testContext
            };

            settings.DownloadPathDir = settings.DownloadPathDir.Replace(".\\", "");

            performanceFilePath = Path.Combine(settings.DataPathDir, "Performance//performance_" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
        }
        private IWebDriver GetDriverByBrowser(string browser)
        {
            switch (browser)
            {
                case "IE":
                    {
                        var enviroment = Environment.GetEnvironmentVariable("ChromeWebDriver");

                        if (enviroment != null)
                        {
                            return new InternetExplorerDriver(enviroment);
                        }
                        else
                        {
                            return new InternetExplorerDriver();
                        }
                    }
                case "Chrome":
                default:
                    {
                        var enviroment = Environment.GetEnvironmentVariable("ChromeWebDriver");
                        var chromeOptions = new OpenQA.Selenium.Chrome.ChromeOptions();

                        chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                        chromeOptions.AddUserProfilePreference("download.default_directory", settings.DownloadPathDir);
                        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                        chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
                        chromeOptions.AddArguments("test-type");
                        chromeOptions.AddArguments("enable-automation");
                        chromeOptions.AddArguments("test-type=browser");
                        chromeOptions.AddArguments("disable-infobars");

                        if (enviroment != null)
                        {
                            return new ChromeDriver(enviroment, chromeOptions);
                        }
                        else
                        {
                            return new ChromeDriver(chromeOptions);
                        }
                    }
            }
        }
        private IWebDriver GetRemoteDriver(string RemoteURL)
        {

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--allow-running-insecure-content");
            chromeOptions.AddArgument("--disable-extensions");

            return new RemoteWebDriver(new Uri(RemoteURL), chromeOptions);
        }
        [TestCleanup()]
        public virtual void TestCleanup()
        {
            WritePerformance();
            if (TestContext.TestName.Contains(seleniumTest) && prepareSeleniumEnv) // only for UI testcases, not for WebApi
            {
                if (TestContext.CurrentTestOutcome == UnitTestOutcome.Failed)
                {
                    var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                    var path = Path.Combine(settings.DownloadPathDir, TestContext.TestName + "_fail_" + dateToday + ".png");
                    screenshot.SaveAsFile(path);
                    TestContext.AddResultFile(path);
                }

                driver.Quit();
                driver.Dispose();
            }
            FlushPerformanceData();
            AddTestCleanup();
            FlushPerformanceData();
            TestContext.AddResultFile(performanceFilePath);
        }
        private void FlushPerformanceData()
        {
            File.AppendAllLines(performanceFilePath, performanceOutputList.ToArray());
            performanceOutputList = new List<string>();
        }
        private void WritePerformance()
        {
            var SEPARATOR = ";";
            var line = TestContext.TestName + SEPARATOR + "test time" + SEPARATOR + "" + SEPARATOR + TestContext.CurrentTestOutcome.ToString() + SEPARATOR;
            line = line + startTest.ToString() + SEPARATOR + DateTime.Now.ToString() + SEPARATOR + (DateTime.Now - startTest) + SEPARATOR;
            performanceOutputList.Add(line);
        }
        public virtual void AddTestCleanup()
        {
        }
        ///<summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContext;
            }
            set
            {
                testContext = value;
            }
        }
    }
}