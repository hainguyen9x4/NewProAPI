using System.Net.NetworkInformation;

namespace Pro.Common
{
    public class WaitForInternetAccess
    {
        private bool IsInternetAccess()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void WaitInternetAccess(string name)
        {
            while (IsInternetAccess() == false)
            {
                LogHelper.Error($"WaitInternetAccess: {name}");
                Thread.Sleep(5000);
            }
        }
    }
}
