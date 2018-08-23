using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkWhitelist;

namespace Test
{
    [TestClass]
    public class DetectorTest
    {
        [TestMethod]
        public void TestOnIPv4()
        {
            Assert.IsTrue(Detector.IsIPv4Protocol("1.0.0.0"));
            Assert.IsTrue(Detector.IsIPv4Protocol("114.114.141.29"));
            Assert.IsTrue(Detector.IsIPv4Protocol("127.0.0.1"));
            Assert.IsTrue(Detector.IsIPv4Protocol("192.0.0.1"));
            Assert.IsTrue(Detector.IsIPv4Protocol("192.168.0.1"));
            Assert.IsTrue(Detector.IsIPv4Protocol("28.8.28.88"));
            Assert.IsTrue(Detector.IsIPv4Protocol("37.32.26.7"));
            Assert.IsTrue(Detector.IsIPv4Protocol("8.8.8.8"));
            Assert.IsFalse(Detector.IsIPv4Protocol("114.114.141.291"));
            Assert.IsFalse(Detector.IsIPv4Protocol("15.1616.1717.17"));
            Assert.IsFalse(Detector.IsIPv4Protocol("256.0.0.0"));
        }

        [TestMethod]
        public void TestOnIPv6()
        {
            Assert.IsTrue(Detector.IsIPv6Protocol("::"));
            Assert.IsTrue(Detector.IsIPv6Protocol("::00:192.168.10.184"));
            Assert.IsTrue(Detector.IsIPv6Protocol("::1"));
            Assert.IsTrue(Detector.IsIPv6Protocol("ae34:ae:fe:12:51:5af:bcde:123"));
            Assert.IsTrue(Detector.IsIPv6Protocol("fe80::219:7eff:fe46:6c42"));
        }

        [TestMethod]
        public void TestOnDetectProtocol()
        {
            var p1 = Detector.DetectProtocol("192.168.0.1");
            var p2 = Detector.DetectProtocol("ae34:ae:fe:12:51:5af:bcde:123");
            Assert.AreEqual(Detector.DetectProtocol("192.168.0.1"), Protocol.IPv4);
            Assert.AreEqual(Detector.DetectProtocol("ae34:ae:fe:12:51:5af:bcde:123"), Protocol.IPv6);
            Assert.AreEqual(Detector.DetectProtocol("114.114.141.291"), Protocol.Invalid);
        }
    }
}