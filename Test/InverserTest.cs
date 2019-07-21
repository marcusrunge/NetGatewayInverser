using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGatewayInverser;

namespace Test
{
    [TestClass]
    public class InverserTest
    {
        [TestMethod]
        public void GetWhiteListTest()
        {
            List<Network> blackList = new List<Network>
            {
                new Network() { Address = "5.9.167.178", Prefix = 32 },
                new Network() { Address = "5.9.167.179", Prefix = 32 },
                new Network() { Address = "5.9.167.180", Prefix = 32 },
                new Network() { Address = "5.9.167.181", Prefix = 32 },
                new Network() { Address = "5.9.167.178", Prefix = 32 },
                new Network() { Address = "5.9.167.179", Prefix = 32 },
                new Network() { Address = "5.9.167.180", Prefix = 32 },
                new Network() { Address = "5.9.167.181", Prefix = 32 },
                new Network() { Address = "10.3.4.167", Prefix = 32 }
                ,new Network() { Address = "0000:0000:0000:0000:0000:0000:0000:0001", Prefix = 128}
                ,new Network() { Address = "FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFE", Prefix = 128}
            };
            var whiteList = Inverser.GetVpnGateways(blackList);
            foreach (var white in whiteList)
            {
                Debug.WriteLine(white.Address + "/" + white.Prefix);
            }
            Assert.IsNotNull(whiteList);
        }
    }
}
