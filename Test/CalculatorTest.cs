using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkWhitelist;

namespace Test
{
    [TestClass]
    public class CalculatorTest
    {
        [TestMethod]
        public void GetWhiteListTest()
        {
            List<Network> blackList = new List<Network>
            {
                new Network() { Address = "127.0.0.1", Prefix = 32, Protocol= Protocol.IPv4 },
                new Network() { Address = "192.168.1.0", Prefix = 24 , Protocol= Protocol.IPv4},
                new Network() { Address = "FE80:0000:0000:0000:DEAD:BEFF:FEEF:CAFE", Prefix = 64, Protocol = Protocol.IPv6},
                new Network() { Address= "2222:3333:4444:5555:0000:0000:6060:0707",  Prefix = 56, Protocol = Protocol.IPv6}
            };
            var whiteList = Calculator.GetWhiteList(blackList);
            Assert.IsNotNull(whiteList);
        }
    }
}
