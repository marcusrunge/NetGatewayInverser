using System;
using System.Collections.Generic;
using System.Numerics;

namespace NetGatewayInverser
{
    public static class Inverser
    {
        private const long IPV4_ADDRESS_SPACE = 4294967296;
        //340282366920938463463374607431768211456        
        private static readonly BigInteger IPV6_ADDRESS_SPACE = BigInteger.Parse("340282366920938463463374607431768211456");
        /// <summary>
        /// The method calculates an IPv4 and IPv6 address space in form of a network list exluding the Net Gateways.
        /// </summary>
        /// <param name="netGateways">
        /// Parameter netGateways requires a list of networks to be excluded from IPv4 and IPv6 address space
        /// </param>
        /// <returns>
        /// The method returns the IPv4 and IPv6 address space as a network list excluding the Net Gateways
        /// </returns>
        public static List<Network> GetVpnGateways(List<Network> netGateways)
        {
            List<Network> vpnGateways = new List<Network>();
            List<Network> cleanedNetGateways = new List<Network>();
            long remainingIPv4 = IPV4_ADDRESS_SPACE;
            BigInteger remainingIPv6 = IPV6_ADDRESS_SPACE;
            long ipv4VpnGateway = 0;
            BigInteger ipv6VpnGateway = BigInteger.Zero;
            long ipv4Networks = 0;
            long ipv6Networks = 0;
            int lastIPv4Iteration = -1;
            int lastIPv6Iteration = -1;

            foreach (var loopOneNetwork in netGateways)
            {
                if (cleanedNetGateways.Count > 0)
                {
                    bool doubleFound = false;
                    foreach (var loopTwoNetwork in cleanedNetGateways)
                    {
                        if (loopOneNetwork.Address.Equals(loopTwoNetwork.Address))
                        {
                            doubleFound = true;
                            break;
                        }
                    }
                    if(!doubleFound) cleanedNetGateways.Add(loopOneNetwork);
                }
                else cleanedNetGateways.Add(loopOneNetwork);
            }

            cleanedNetGateways.Sort();
                        

            for (int i = 0; i < cleanedNetGateways.Count; i++)
            {
                if (Detector.IsIPv4Protocol(cleanedNetGateways[i].Address))
                {
                    long netGateway = Converter.ConvertToLongAddress(cleanedNetGateways[i].Address);
                    long netGatewayBroadcast = netGateway + (long)Math.Pow(2, 32 - cleanedNetGateways[i].Prefix) - 1;

                    if (i > 0 && lastIPv4Iteration > -1)
                    {
                        ipv4VpnGateway = Converter.ConvertToLongAddress(cleanedNetGateways[lastIPv4Iteration].Address) + (long)Math.Pow(2, 32 - cleanedNetGateways[lastIPv4Iteration].Prefix);
                    }

                    long remainingVpnGatewaySpace = netGateway - ipv4VpnGateway;
                    while (remainingVpnGatewaySpace > 0)
                    {
                        int prefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingVpnGatewaySpace) / Math.Log10(2)));
                        long vpnGatewaySize = (long)Math.Pow(2, 32 - prefixLength);
                        long border = (long)(Math.Floor((double)(ipv4VpnGateway + vpnGatewaySize) / vpnGatewaySize) * vpnGatewaySize);
                        //No border violation
                        if (ipv4VpnGateway + vpnGatewaySize <= border)
                        {
                            vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv4Address(ipv4VpnGateway), Prefix = prefixLength });
                            ipv4Networks++;
                            ipv4VpnGateway = ipv4VpnGateway + vpnGatewaySize;
                            remainingVpnGatewaySpace = remainingVpnGatewaySpace - vpnGatewaySize;
                            //Border violation
                        }
                        else
                        {
                            long left = border - ipv4VpnGateway;
                            long remainingLeft = left;
                            long shiftBorder = border;
                            while (remainingLeft > 0)
                            {
                                int leftPrefixLength = (int)Math.Ceiling(32 - (Math.Log10(remainingLeft) / Math.Log10(2)));
                                long shiftLeft = (long)Math.Pow(2, 32 - leftPrefixLength);
                                shiftBorder = shiftBorder - shiftLeft;
                                vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv4Address(shiftBorder), Prefix = leftPrefixLength });                                
                                ipv4Networks++;
                                remainingLeft = remainingLeft - shiftLeft;
                            }
                            ipv4VpnGateway = border;
                            remainingVpnGatewaySpace = remainingVpnGatewaySpace - left;
                        }
                    }
                    remainingIPv4 = IPV4_ADDRESS_SPACE - netGatewayBroadcast - 1;
                    lastIPv4Iteration = i;
                }
                else if (Detector.IsIPv6Protocol(cleanedNetGateways[i].Address))
                {                       
                    BigInteger netGateway = Converter.ConvertToBigIntegerAddress(cleanedNetGateways[i].Address);
                    BigInteger netGatewayBroadcast = netGateway + BigInteger.Pow(2, 128 - cleanedNetGateways[i].Prefix) - 1;
                    if (i > 0 && lastIPv6Iteration > -1)
                    {
                        ipv6VpnGateway = Converter.ConvertToBigIntegerAddress(cleanedNetGateways[lastIPv6Iteration].Address) + BigInteger.Pow(2, 128 - cleanedNetGateways[lastIPv6Iteration].Prefix);
                    }
                    BigInteger remainingVpnGatewaySpace = netGateway - ipv6VpnGateway;

                    while (remainingVpnGatewaySpace > 0)
                    {
                        int prefixLength = (int)Math.Ceiling(128 - (BigInteger.Log10(remainingVpnGatewaySpace) / Math.Log10(2)));
                        BigInteger vpnGatewaySize = BigInteger.Pow(2, 128 - prefixLength);
                        BigInteger border = BigInteger.Divide(ipv6VpnGateway + vpnGatewaySize, vpnGatewaySize) * vpnGatewaySize;
                        //No border violation
                        if (ipv6VpnGateway + vpnGatewaySize <= border)
                        {
                            vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv6Address(ipv6VpnGateway), Prefix = prefixLength });
                            ipv6Networks++;
                            ipv6VpnGateway = ipv6VpnGateway + vpnGatewaySize;
                            remainingVpnGatewaySpace = remainingVpnGatewaySpace - vpnGatewaySize;
                            //Border violation
                        }
                        else
                        {
                            BigInteger left = border - ipv6VpnGateway;
                            BigInteger remainingLeft = left;
                            BigInteger shiftBorder = border;
                            while (remainingLeft > 0)
                            {
                                int leftPrefixLength = (int)Math.Ceiling(128 - (BigInteger.Log10(remainingLeft) / Math.Log10(2)));
                                BigInteger shiftLeft = BigInteger.Pow(2, 128 - leftPrefixLength);
                                shiftBorder = shiftBorder - shiftLeft;
                                vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv6Address(shiftBorder), Prefix = leftPrefixLength });
                                ipv6Networks++;
                                remainingLeft = remainingLeft - shiftLeft;
                            }                            
                            ipv6VpnGateway = border;
                            remainingVpnGatewaySpace = remainingVpnGatewaySpace - left;
                        }
                    }
                    remainingIPv6 = IPV6_ADDRESS_SPACE - netGatewayBroadcast - 1;

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
                    vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv4Address(lastNetwork), Prefix = prefixLength });
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
                    vpnGateways.Add(new Network() { Address = Converter.ConvertToIPv6Address(lastNetwork), Prefix = prefixLength });
                    ipv6Networks++;
                    remainingIPv6 = remainingIPv6 - lastNetworkSize;
                }
            }            

            return vpnGateways;
        }
    }
}