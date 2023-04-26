using Pro.Common;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.Arm;

namespace UnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetImageDatasFromWeb()
        {
            var acc =new WaitForInternetAccess();
            //acc.WaitInternetAccess();
            var urlStory = "https://www.nettruyenvi.com/truyen-tranh/su-tro-lai-ma-duoc-su-cap-fff-82543";
            var urlBase = "https://xstoryfunction.azurewebsites.net";
            var rs = new ApiHelper().Post<string>($"/api/GetPictureLinkFormStoryLinkByAPI?url={urlStory}", null, urlBase);
            Assert.Pass();
        }
    }
}