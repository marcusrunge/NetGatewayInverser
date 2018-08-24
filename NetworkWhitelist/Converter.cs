using System;
using System.Collections.Generic;

namespace NetworkWhitelist
{
    internal static class Converter
    {
        /// <summary>
        /// The method converts a numerical represented IPv4 address eg. "2130706433" into a string representation eg. "127.0.0.1"
        /// </summary>
        /// <param name="address">
        /// Parameter address require the numerical represented IPv4 address eg. "2130706433"
        /// </param>
        /// <returns>
        /// The method returns the string represented IPv4 address eg. "127.0.0.1"
        /// </returns>
        internal static string ConvertToAddress(long address)
        {
            int[] octets = new int[4];
            int exponent = 3;
            for (int i = 0; i < 4; i++)
            {
                long polynom = (long)Math.Pow(256, exponent);
                int octet = (int)(address / polynom);
                octets[i] = octet;
                address = address - (long)octet * polynom;
                exponent--;
            }
            return octets[0] + "." + octets[1] + "." + octets[2] + "." + octets[3];
        }

        /// <summary>
        /// The method converts a string formatted IPv4 address eg. "127.0.0.1" into a numerical representation
        /// </summary>
        /// <param name="address">
        /// Parameter address require the string formatted IPv4 address eg. "127.0.0.1"
        /// </param>
        /// <returns>
        /// The method returns the IPv4 address as a numerical representation
        /// </returns>
        internal static long ConvertToLongAddress(string address)
        {
            int[] octetsAsInt = new int[4];
            String[] octetsAsString = address.Split('.');
            for (int i = 0; i < 4; i++)
            {
                if (int.TryParse(octetsAsString[i], out int octet)) octetsAsInt[i] = octet;
                else return -1;
            }
            return (long)octetsAsInt[0] * (long)16777216 + (long)octetsAsInt[1] * (long)65536 + (long)octetsAsInt[2] * (long)256 + (long)octetsAsInt[3];
        }

        /// <summary>
        /// The method converts a string formatted IPv6 address eg. "::ffff:7f00:1" into a blockwise numerical representation
        /// </summary>
        /// <param name="ipv6Address">
        /// Parameter address require the string formatted IPv6 address eg. "::ffff:7f00:1"
        /// </param>
        /// <returns>
        /// The method returns the IPv6 address as a blockwise numerical representation
        /// </returns>
        internal static long[] ConvertToLongIPv6Blocks(string ipv6Address)
        {
            long[] ipv6BlocksAsLong = new long[8];
            string[] ipv6BlocksAsString = ipv6Address.Split(':');
            for (int i = 0; i < 8; i++)
            {
                if (ipv6BlocksAsString.Length >= i + 1)
                {
                    if (ipv6BlocksAsString[i].Equals("")) ipv6BlocksAsLong[i] = 0;
                    else ipv6BlocksAsLong[i] = Convert.ToInt64(ipv6BlocksAsString[i], 16);
                }
                else ipv6BlocksAsLong[i] = 0;
            }
            return ipv6BlocksAsLong;
        }

        /// <summary>
        /// The method converts a blockwise numerical formatted IPv6 address into a string representation
        /// </summary>
        /// <param name="ipv6Blocks">
        /// Parameter address require the blockwise numerical formatted IPv6 address
        /// </param>
        /// <returns>
        /// The method returns the IPv6 address as a hex string representation
        /// </returns>
        internal static string ConvertToIPv6Address(long[] ipv6Blocks)
        {
            string ipv6Address = String.Empty;
            for (int i = 0; i < 8; i++)
            {
                if (ipv6Blocks.Length >= i + 1)
                {
                    ipv6Address = ipv6Address + ":" + ipv6Blocks[i];
                }
                else ipv6Address = ipv6Address + ":0";
            }
            ipv6Address = ipv6Address.Remove(0, 1);
            return ipv6Address;
        }

        /// <summary>
        /// The method converts a numerical value into a hexadecimal string
        /// </summary>
        /// <param name="value">
        /// Parameter value require the numerical value
        /// </param>
        /// <returns>
        /// The method returns the a hexadecimal string representation
        /// </returns>
        internal static String ConvertToHexString(long value)
        {
            string[] hex = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            List<string> literals = new List<string>();
            string hexString = string.Empty;
            long remainder = 0;
            while (value > 0)
            {
                remainder = value % 16;
                value = value / 16;
                literals.Add(hex[remainder]);
            }
            literals.Reverse();
            literals.ForEach((s) => hexString = hexString + s);
            return hexString;
        }
    }
}