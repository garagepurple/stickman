using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace websockettransmittertest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void StartServer()
        {
            websockettransmitter.WebSocketHost host = new websockettransmitter.WebSocketHost();
            Assert.IsTrue(host.Setup());
            Thread.Sleep(5000);
            Assert.IsTrue(host.IsCloudConnected());
            // Send some test data.
            // The data will be send to the cloud
            // as well as to the locally connected websocket clients
            host.Send("Send");
            Thread.Sleep(5000);
            host.Send("some");
            Thread.Sleep(5000);
            host.Send("test");
            Thread.Sleep(5000);
            host.Send("data");
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
