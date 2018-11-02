using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetworkWhitelist
{
    public static class Calculator
    {
        private const long IPV4_ADDRESS_SPACE = 4294967296;
        //340282366920938463463374607431768211456        
        private static readonly BigInteger IPV6_ADDRESS_SPACE = BigInteger.Parse("340282366920938463463374607431768211456");
        /// <summary>
        /// The method calculates an IPv4 and IPv6 address space in form of a network list exluding the blacklist.
        /// </summary>
        /// <param name="cleanedBlackList">
        /// Parameter blacklist requires a list of networks to be excluded from IPv4 and IPv6 address space
        /// </param>
        /// <returns>
        /// The method returns the IPv4 and IPv6 address space as a network list excluding the blacklisted
        /// </returns>
        public static List<Network> GetWhiteList(List<Network> rawBlackList)
        {
            List<Network> whiteList = new List<Network>();
            List<Network> cleanedBlackList = new List<Network>();
            long remainingIPv4 = IPV4_ADDRESS_SPACE;
            BigInteger remainingIPv6 = IPV6_ADDRESS_SPACE;
            long ipv4WhiteListNetwork = 0;
            BigInteger ipv6WhiteListNetwork = BigInteger.Zero;
            long ipv4Networks = 0;
            long ipv6Networks = 0;
            int lastIPv4Iteration = -1;
            int lastIPv6Iteration = -1;

            foreach (var loopOneNetwork in rawBlackList)
            {
                if (cleanedBlackList.Count > 0)
                {
                    bool doubleFound = false;
                    foreach (var loopTwoNetwork in cleanedBlackList)
                    {
                        if (loopOneNetwork.Address.Equals(loopTwoNetwork.Address))
                        {
                            doubleFound = true;
                            break;
                        }
                    }
                    if(!doubleFound) cleanedBlackList.Add(loopOneNetwork);
                }
                else cleanedBlackList.Add(loopOneNetwork);
            }

            cleanedBlackList.Sort();

            if (cleanedBlackList == null || cleanedBlackList.Count == 0)
            {
                whiteList.Add(new Network() { Address = "0.0.0.0", Prefix = 0 });
                ipv4Networks++;
                whiteList.Add(new Network() { Address = "::", Prefix = 0 });
                ipv6Networks++;
            }

            for (int i = 0; i < cleanedBlackList.Count; i++)
            {
                if (Detector.IsIPv4Protocol(cleanedBlackList[i].Address))
                {
                    long blackListNetwork = Converter.ConvertToLongAddress(cleanedBlackList[i].Address);
                    long blackListNetworkBroadcast = blackListNetwork + (long)Math.Pow(2, 32 - cleanedBlackList[i].Prefix) - 1;

                    if (i > 0 && lastIPv4Iteration > -1)
                    {
                        ipv4WhiteListNetwork = Converter.ConvertToLongAddress(cleanedBlackList[lastIPv4Iteration].Address) + (long)Math.Pow(2, 32 - cleanedBlackList[lastIPv4Iteration].Prefix);
                    }

                    long remainingWhiteListSpace = blackListNetwork - ipv4WhiteListNetwork;
                    while (remainingWhiteListSpace > 0)
                    {
                        int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingWhiteListSpace) / Math.Log10(2)));
                        long whiteListNetworkSize = (long)Math.Pow(2, 32 - prefixLength);
                        long border = (long)(Math.Floor((double)(ipv4WhiteListNetwork + whiteListNetworkSize) / whiteListNetworkSize) * whiteListNetworkSize);
                        //No border violation
                        if (ipv4WhiteListNetwork + whiteListNetworkSize <= border)
                        {
                            whiteList.Add(new Network() { Address = Converter.ConvertToIPv4Address(ipv4WhiteListNetwork), Prefix = prefixLength });
                            ipv4Networks++;
                            ipv4WhiteListNetwork = ipv4WhiteListNetwork + whiteListNetworkSize;
                            remainingWhiteListSpace = remainingWhiteListSpace - whiteListNetworkSize;
                            //Border violation
                        }
                        else
                        {
                            long left = border - ipv4WhiteListNetwork;
                            int leftPrefixLength = (int)Math.Floor(32 - (Math.Log10(left) / Math.Log10(2)));
                            whiteList.Add(new Network() { Address = Converter.ConvertToIPv4Address(border - left), Prefix = leftPrefixLength });
                            ipv4Networks++;
                            ipv4WhiteListNetwork = border;
                            remainingWhiteListSpace = remainingWhiteListSpace - left;
                        }
                    }
                    remainingIPv4 = IPV4_ADDRESS_SPACE - blackListNetworkBroadcast - 1;
                    lastIPv4Iteration = i;
                }
                else if (Detector.IsIPv6Protocol(cleanedBlackList[i].Address))
                {                       
                    BigInteger blackListNetwork = Converter.ConvertToBigIntegerAddress(cleanedBlackList[i].Address);
                    BigInteger blackListNetworkBroadcast = blackListNetwork + BigInteger.Pow(2, 128 - cleanedBlackList[i].Prefix) - 1;
                    if (i > 0 && lastIPv6Iteration > -1)
                    {
                        ipv6WhiteListNetwork = Converter.ConvertToBigIntegerAddress(cleanedBlackList[lastIPv6Iteration].Address) + BigInteger.Pow(2, 128 - cleanedBlackList[lastIPv6Iteration].Prefix);
                    }
                    BigInteger remainingWhiteListSpace = blackListNetwork - ipv6WhiteListNetwork;

                    while (remainingWhiteListSpace > 0)
                    {
                        int prefixLength = (int)Math.Ceiling(128 - (BigInteger.Log10(remainingWhiteListSpace) / Math.Log10(2)));
                        BigInteger whiteListNetworkSize = BigInteger.Pow(2, 128 - prefixLength);
                        BigInteger border = BigInteger.Divide(ipv6WhiteListNetwork + whiteListNetworkSize, whiteListNetworkSize) * whiteListNetworkSize;
                        //No border violation
                        if (ipv6WhiteListNetwork + whiteListNetworkSize <= border)
                        {
                            whiteList.Add(new Network() { Address = Converter.ConvertToIPv6Address(ipv6WhiteListNetwork), Prefix = prefixLength });
                            ipv6Networks++;
                            ipv6WhiteListNetwork = ipv6WhiteListNetwork + whiteListNetworkSize;
                            remainingWhiteListSpace = remainingWhiteListSpace - whiteListNetworkSize;
                            //Border violation
                        }
                        else
                        {
                            BigInteger left = border - ipv6WhiteListNetwork;
                            int leftPrefixLength = (int)Math.Floor(128 - (BigInteger.Log10(left) / Math.Log10(2)));
                            whiteList.Add(new Network() { Address = Converter.ConvertToIPv6Address(border - left), Prefix = leftPrefixLength });
                            ipv6Networks++;
                            ipv6WhiteListNetwork = border;
                            remainingWhiteListSpace = remainingWhiteListSpace - left;
                        }
                    }
                    remainingIPv6 = IPV6_ADDRESS_SPACE - blackListNetworkBroadcast - 1;

                    lastIPv6Iteration = i;
                }
            }

            //Finalize
            if (remainingIPv4 > 0 && lastIPv4Iteration > -1)
            {
                long lastNetwork = IPV4_ADDRESS_SPACE;
                while (remainingIPv4 > 0)
                {
                    int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingIPv4) / Math.Log10(2)));
                    long lastNetworkSize = (long)Math.Pow(2, 32 - prefixLength);
                    lastNetwork = lastNetwork - lastNetworkSize;
                    whiteList.Add(new Network() { Address = Converter.ConvertToIPv4Address(lastNetwork), Prefix = prefixLength });
                    ipv4Networks++;
                    remainingIPv4 = remainingIPv4 - lastNetworkSize;
                }
            }

            if (remainingIPv6 > 0 && lastIPv6Iteration > -1)
            {
                BigInteger lastNetwork = IPV6_ADDRESS_SPACE;
                while (remainingIPv6 > 0)
                {
                    int prefixLength = (int)Math.Ceiling(128 - (BigInteger.Log10(remainingIPv6) / Math.Log10(2)));
                    BigInteger lastNetworkSize = BigInteger.Pow(2, 128 - prefixLength);
                    lastNetwork = lastNetwork - lastNetworkSize;
                    whiteList.Add(new Network() { Address = Converter.ConvertToIPv6Address(lastNetwork), Prefix = prefixLength });
                    ipv6Networks++;
                    remainingIPv6 = remainingIPv6 - lastNetworkSize;
                }
            }

            //if (ipv4Networks == 0) whiteList.Add(new Network() { Address = "0.0.0.0", Prefix = 0, Protocol = Protocol.IPv4 });
            //if (ipv6Networks == 0) whiteList.Add(new Network() { Address = "::", Prefix = 0, Protocol = Protocol.IPv6 });

            return whiteList;
        }
    }
}