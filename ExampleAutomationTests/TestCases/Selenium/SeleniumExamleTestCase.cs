using Microsoft.VisualStudio.TestTools.UnitTesting;
using mySpaceName.Helpers.WebDriver;

namespace mySpaceName.TestCases
{
    [TestClass]
    public class SeleniumExamleTestCase : BaseTest
    {
        /**
         * add code what is necessary have before tescase - for example data in DB
         **/
        public override void AddTestInitialize()
        {
        }

        /**
         * testcase
         **/
        [TestCategory("Selenium")]
        [TestMethod]
        public void Selenium_ExampleTest()
        {
            var elemText = context.GetTextElement(".flash.flash-full.flash-error");
            Assert.AreEqual(elemText, "Incorrect username or password.");

        }

        /**
         * clean data after testcase run
         **/
        public override void AddTestCleanup()
        {

        }
    }
}