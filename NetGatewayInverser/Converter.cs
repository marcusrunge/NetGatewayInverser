using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NetGatewayInverser
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
        internal static string ConvertToIPv4Address(long address)
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

        /// <summary>
        /// The method converts a string formatted IPv6 address eg. "::ffff:7f00:1" into a decimal representation
        /// </summary>
        /// <param name="ipv6Address">
        /// Parameter address require the string formatted IPv6 address eg. "::ffff:7f00:1"
        /// </param>
        /// <returns>
        /// The method returns the IPv6 address as a decimal representation
        /// </returns>
        internal static BigInteger ConvertToBigIntegerAddress(string ipv6Address)
        {
            BigInteger ipv6AsBigInteger = new BigInteger();
            string[] ipv6BlocksAsStringCorrected = new string[8];
            if (ipv6Address.Equals("::")) return new BigInteger(0);
            if (ipv6Address.Length < 32)
            {
                string[] leadingZeroBlocks;
                bool leadingZeros = ipv6Address.StartsWith("::");
                string[] ipv6BlocksAsString = ipv6Address.Split(':');
                if (leadingZeros)
                {
                    leadingZeroBlocks = new string[8 - ipv6BlocksAsString.Length];
                    for (int i = 0; i < leadingZeroBlocks.Length; i++)
                    {
                        leadingZeroBlocks[i] = "0000";
                    }
                    ipv6BlocksAsString = leadingZeroBlocks.ToList().Concat(ipv6BlocksAsString.ToList()).ToArray();
                }
                for (int i = 0; i < 8; i++)
                {
                    if (ipv6BlocksAsString.Length >= i + 1)
                    {
                        for (int j = 0; j < 4 - ipv6BlocksAsString[i].Length; j++)
                        {
                            ipv6BlocksAsStringCorrected[i] = ipv6BlocksAsStringCorrected[i] + "0";
                        }
                        ipv6BlocksAsStringCorrected[i] = ipv6BlocksAsStringCorrected[i] + ipv6BlocksAsString[i];
                    }
                    else ipv6BlocksAsStringCorrected[i] = "0000";
                }
                ipv6Address = string.Empty;
                foreach (var block in ipv6BlocksAsStringCorrected) ipv6Address = ipv6Address + block;
            }
            else ipv6Address = ipv6Address.Replace(":", "");
            int e = 31;
            for (int i = 0; i < ipv6Address.Length; i++)
            {
                int basePart = int.Parse(ipv6Address[i].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                ipv6AsBigInteger = ipv6AsBigInteger + (basePart * BigInteger.Pow(16, e));
                e--;
            }
            return ipv6AsBigInteger;
        }

        /// <summary>
        /// The method converts a decimal formatted IPv6 address eg. "281472812449793" into a hexadecimal IPv6 representation
        /// </summary>
        /// <param name="ipv6AsBigInteger">
        /// Parameter address require the decimal IPv6 address eg. "281472812449793"
        /// </param>
        /// <returns>
        /// The method returns the IPv6 address as a hexadecimal IPv6 representation
        /// </returns>
        internal static string ConvertToIPv6Address(BigInteger ipv6AsBigInteger)
        {
            string[] hex = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            List<string> literals = new List<string>();
            BigInteger remainder = new BigInteger(0);
            while (ipv6AsBigInteger > 0)
            {
                remainder = ipv6AsBigInteger % 16;
                ipv6AsBigInteger = ipv6AsBigInteger / 16;
                literals.Add(hex[(int)remainder]);
            }
            int literalsCount = literals.Count;
            if (literalsCount < 32) for (int i = 0; i < 32 - literalsCount; i++) literals.Add("0");
            string ipv6Address = String.Empty;
            literals.Reverse();
            int j = 0;
            literals.ForEach((s) =>
            {
                ipv6Address = ipv6Address + s;
                if (j == 3)
                {
                    ipv6Address = ipv6Address + ":";
                    j = -1;
                }
                j++;
            });
            return ipv6Address.TrimEnd(':');
        }
    }
}