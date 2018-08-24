using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetworkWhitelist
{
    public static class Calculator
    {
        private const long IPV4_ADDRESS_SPACE = 4294967296;
        //340282366920938463463374607431768211456
        private static readonly BigInteger IPV6 = BigInteger.Pow(2, 128);
        /// <summary>
        /// The method calculates an IPv4 address space in form of a network list exluding the blacklist.
        /// </summary>
        /// <param name="blackList">
        /// Parameter blacklist requires a list of networks to be excluded from IPv4 address space
        /// </param>
        /// <returns>
        /// The method returns the IPv4 address space as a network list excluding the blacklisted
        /// </returns>
        public static List<Network> GetWhiteList(List<Network> blackList)
        {
            List<Network> whiteList = new List<Network>();
            long remainingIPv4 = IPV4_ADDRESS_SPACE;
            Tuple<int, long> remainingIPv6 = new Tuple<int, long>(128, 0);
            long ipv4WhiteListNetwork = 0;
            long[] ipv6WhiteListNetwork = new long[8];
            long ipv4Networks = 0;
            long ipv6Networks = 0;

            blackList.Sort();
            if (blackList == null || blackList.Count == 0)
            {
                whiteList.Add(new Network() { Address = "0.0.0.0", Prefix = 0 });
                ipv4Networks++;
                whiteList.Add(new Network() { Address = "::", Prefix = 0 });
                ipv6Networks++;
            }


            for (int i = 0; i < blackList.Count; i++)
            {
                if (Detector.IsIPv4Protocol(blackList[i].Address))
                {
                    long blackListNetwork = Converter.ConvertToLongAddress(blackList[i].Address);
                    long blackListNetworkBroadcast = blackListNetwork + (long)Math.Pow(2, 32 - blackList[i].Prefix) - 1;

                    if (i > 0)
                    {
                        ipv4WhiteListNetwork = Converter.ConvertToLongAddress(blackList[i - 1].Address) + (long)Math.Pow(2, 32 - blackList[i - 1].Prefix);
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
                            whiteList.Add(new Network() { Address = Converter.ConvertToAddress(ipv4WhiteListNetwork), Prefix = prefixLength });
                            ipv4Networks++;
                            ipv4WhiteListNetwork = ipv4WhiteListNetwork + whiteListNetworkSize;
                            remainingWhiteListSpace = remainingWhiteListSpace - whiteListNetworkSize;
                            //Border violation
                        }
                        else
                        {
                            long left = border - ipv4WhiteListNetwork;
                            int leftPrefixLength = (int)Math.Floor(32 - (Math.Log10(left) / Math.Log10(2)));
                            whiteList.Add(new Network() { Address = Converter.ConvertToAddress(border - left), Prefix = leftPrefixLength });
                            ipv4Networks++;
                            ipv4WhiteListNetwork = border;
                            remainingWhiteListSpace = remainingWhiteListSpace - left;
                        }
                    }
                    remainingIPv4 = IPV4_ADDRESS_SPACE - blackListNetworkBroadcast - 1;
                }
                else if (Detector.IsIPv6Protocol(blackList[i].Address))
                {
                    //TODO
                    var ipv6BlackListBlocks = Converter.ConvertToLongIPv6Blocks(blackList[i].Address);
                    
                }
            }

            //Finalize
            if (remainingIPv4 > 0)
            {
                long lastNetwork = IPV4_ADDRESS_SPACE;
                while (remainingIPv4 > 0)
                {
                    int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingIPv4) / Math.Log10(2)));
                    long lastNetworkSize = (long)Math.Pow(2, 32 - prefixLength);
                    lastNetwork = lastNetwork - lastNetworkSize;
                    whiteList.Add(new Network() { Address = Converter.ConvertToAddress(lastNetwork), Prefix = prefixLength });
                    ipv4Networks++;
                    remainingIPv4 = remainingIPv4 - lastNetworkSize;
                }
            }

            if (ipv4Networks == 0)  whiteList.Add(new Network() { Address = "0.0.0.0", Prefix = 0 });
            if (ipv6Networks == 0) whiteList.Add(new Network() { Address = "::", Prefix = 0 });
            
            return whiteList;
        } 
    }
}