using System;

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
    }    
}