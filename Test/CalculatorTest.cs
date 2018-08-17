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
                new Network() { Address = "127.0.0.1", Prefix = 32 },
                new Network() { Address = "192.168.1.0", Prefix = 24 }
            };
            var whiteList = Calculator.GetWhiteList(blackList);
            Assert.IsNotNull(whiteList);
        }
    }
}
