using System;

namespace NetworkWhitelist
{
    public class Network : IComparable<Network>
    {
        public string Address { get; set; }
        public int Prefix { get; set; }
        public Protocol Protocol { get; set; }

        public int CompareTo(Network other)
        {
            if (other == null) return 1;
            return other.Address.CompareTo(Address);
        }
    }
}