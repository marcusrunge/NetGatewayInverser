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
                new Network() { Address = "5.9.167.178", Prefix = 32, Protocol= Protocol.IPv4 },
                new Network() { Address = "5.9.167.179", Prefix = 32 , Protocol= Protocol.IPv4},
                new Network() { Address = "5.9.167.180", Prefix = 32 , Protocol= Protocol.IPv4},
                new Network() { Address = "5.9.167.181", Prefix = 32 , Protocol= Protocol.IPv4}                
                ,new Network() { Address = "0000:0000:0000:0000:0000:0000:0000:0001", Prefix = 128, Protocol = Protocol.IPv6}                
                ,new Network() { Address = "FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFF:FFFE", Prefix = 128, Protocol = Protocol.IPv6}
            };
            var whiteList = Calculator.GetWhiteList(blackList);
            Assert.IsNotNull(whiteList);
        }
    }
}
