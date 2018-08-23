using System;
using System.Collections.Generic;

namespace NetworkWhitelist
{
    public static class Calculator
    {
        private const long IPV4_ADDRESS_SPACE = 4294967296;

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
            long remaining = IPV4_ADDRESS_SPACE;
            long whiteListNetwork = 0;            
            blackList.Sort();
            if (blackList == null || blackList.Count == 0) whiteList.Add(new Network() { Address = "0.0.0.0", Prefix = 0 });

            for (int i = 0; i < blackList.Count; i++)
            {
                long blackListNetwork = Converter.ConvertToLongAddress(blackList[i].Address);
                long blackListNetworkBroadcast = blackListNetwork + (long)Math.Pow(2, 32 - blackList[i].Prefix) - 1;

                if (i > 0)
                {
                    whiteListNetwork = Converter.ConvertToLongAddress(blackList[i - 1].Address) + (long)Math.Pow(2, 32 - blackList[i - 1].Prefix);
                }

                long remainingWhiteListSpace = blackListNetwork - whiteListNetwork;
                while (remainingWhiteListSpace > 0)
                {
                    int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingWhiteListSpace) / Math.Log10(2)));
                    long whiteListNetworkSize = (long)Math.Pow(2, 32 - prefixLength);
                    long border = (long)(Math.Floor((double)(whiteListNetwork + whiteListNetworkSize) / whiteListNetworkSize) * whiteListNetworkSize);
                    //No border violation
                    if (whiteListNetwork + whiteListNetworkSize <= border)
                    {
                        whiteList.Add(new Network() { Address = Converter.ConvertToAddress(whiteListNetwork), Prefix = prefixLength });
                        whiteListNetwork = whiteListNetwork + whiteListNetworkSize;
                        remainingWhiteListSpace = remainingWhiteListSpace - whiteListNetworkSize;
                        //Border violation
                    }
                    else
                    {
                        long left = border - whiteListNetwork;
                        int leftPrefixLength = (int)Math.Floor(32 - (Math.Log10(left) / Math.Log10(2)));
                        whiteList.Add(new Network() { Address = Converter.ConvertToAddress(border - left), Prefix = leftPrefixLength });
                        whiteListNetwork = border;
                        remainingWhiteListSpace = remainingWhiteListSpace - left;
                    }
                }
                remaining = IPV4_ADDRESS_SPACE - blackListNetworkBroadcast - 1;
            }

            //Finalize
            if (remaining > 0)
            {
                long lastNetwork = IPV4_ADDRESS_SPACE;
                while (remaining > 0)
                {
                    int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remaining) / Math.Log10(2)));
                    long lastNetworkSize = (long)Math.Pow(2, 32 - prefixLength);
                    lastNetwork = lastNetwork - lastNetworkSize;
                    whiteList.Add(new Network() { Address = Converter.ConvertToAddress(lastNetwork), Prefix = prefixLength });
                    remaining = remaining - lastNetworkSize;
                }
            }
            return whiteList;
        }
    }
}