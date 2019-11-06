using Microsoft.VisualStudio.TestTools.UnitTesting;
using mySpaceName.Helpers.WebDriver;

namespace mySpaceName.TestCases
{
    [TestClass]
    public class WebApiExampleTestCase : BaseTest
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
        [TestCategory("webapi")]
        [TestMethod]
        public void WebAPI_ExampleTest()
        {
            webapi.SendGETRequest("", System.Net.HttpStatusCode.NotAcceptable);
        }

        /**
         * clean data after testcase run
         **/
        public override void AddTestCleanup()
        {
        }
    }
}
