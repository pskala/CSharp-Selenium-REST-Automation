/**
 * common useful selenium operations
 **/
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace mySpaceName.Helpers.Api
{
    public class Context
    {

        public IWebDriver driver;
        private WebDriverWait longWait;
        private WebDriverWait shortWait;
        private string appURL;

        public Context(IWebDriver driver, string appURL)
        {
            this.driver = driver;
            this.appURL = appURL;
            longWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }
        public IWebDriver GetDriver()
        {
            return driver;
        }
        public void SetURL(string hash = "")
        {
            driver.Navigate().GoToUrl(appURL + hash);
        }
        public void RefreshPage()
        {
            driver.Navigate().Refresh();
        }
        public void WaitForClickableElement(By by)
        {
            longWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
        }
        public void WaitLongForVisibleElement(By by)
        {
            longWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }
        public void WaitShortForVisibleElement(By by)
        {
            shortWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }
        public void Click(string css)
        {
            var by = By.CssSelector(css);
            WaitForClickableElement(by);
            driver.FindElement(by).Click();
        }
        public IWebElement GetElementByCss(string css)
        {
            var by = By.CssSelector(css);
            WaitLongForVisibleElement(by);
            return driver.FindElement(by);
        }
        public IWebElement GetElementByInnerTextContains(string text, IWebElement areaLocation = null)
        {
            return GetElementByXpath(".//*[contains(text(), '" + text + "')]", areaLocation);
        }
        public IWebElement GetElementByInnerText(string text, IWebElement areaLocation = null)
        {
            return GetElementByXpath(".//*[text() = '" + text + "']", areaLocation);
        }
        public IWebElement GetElementByXpath(string xpath, IWebElement areaLocation = null)
        {
            var by = By.XPath(xpath);
            longWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
            if (areaLocation == null)
            {
                return driver.FindElement(by);
            } else
            {
                return areaLocation.FindElement(by);
            }
        }
        public string GetTextElement(string css, bool withWaiting = true)
        {
            By by = By.CssSelector(css);
            if (withWaiting)
            {
                WaitLongForVisibleElement(by);
            }
            return driver.FindElement(by).Text;
        }
        public void SetValue(IWebElement element,  object value)
        {
            SetValueForElement(value, element);
        }
        public void SetValue(object value, string css)
        {
            if (value != null)
            {
                var bySelect = By.CssSelector(css);
                WaitForClickableElement(bySelect);
                IWebElement element = driver.FindElement(bySelect);
                SetValueForElement(value, element);
            }
        }
        private void SetValueForElement(object value, IWebElement element)
        {
            element.Clear();
            element.SendKeys(value.ToString());
        }
    }
}
