using System;
using System.Numerics;

namespace NetGatewayInverser
{
    public class Network : IComparable<Network>
    {
        private string address;

        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                Protocol = Detector.DetectProtocol(value);
                switch (Protocol)
                {
                    case Protocol.IPv4:
                        BigIntegerAddress = Converter.ConvertToLongAddress(value);
                        break;
                    case Protocol.IPv6:
                        BigIntegerAddress = Converter.ConvertToBigIntegerAddress(value);
                        break;
                    case Protocol.Invalid:
                        break;
                    default:
                        break;
                }
            }
        }
        
        public BigInteger BigIntegerAddress { get; private set; }
        public int Prefix { get; set; }
        public Protocol Protocol { get; private set; }

        public int CompareTo(Network other)
        {
            if (other == null) return 1;
            return BigIntegerAddress.CompareTo(other.BigIntegerAddress);
        }        
    }
}