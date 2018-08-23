using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkWhitelist
{
    public static class Detector
    {
        static readonly string IPv4_RFC3986 = "\\A(?:25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)(?:\\.(?:25[0-5]|2[0-4]\\d|1\\d\\d|[1-9]\\d|\\d)){3}\\z";
        static readonly string IPv6_RFC2732_v2 = "\\A(?:(?:(?:[0-9A-Fa-f]{0,4}:){7}[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){6}:[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){5}:(?:[0-9A-Fa-f]{0,4}:)?[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){4}:(?:[0-9A-Fa-f]{0,4}:){0,2}[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){3}:(?:[0-9A-Fa-f]{0,4}:){0,3}[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){2}:(?:[0-9A-Fa-f]{0,4}:){0,4}[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){6}(?:(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2}))\\.){3}(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2})))|(?:(?:[0-9A-Fa-f]{0,4}:){0,5}:(?:(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2}))\\.){3}(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2})))|(?:::(?:[0-9A-Fa-f]{0,4}:){0,5}(?:(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2}))\\.){3}(?:(?:25[0-5])|(?:2[0-4]\\d)|(?:1\\d{2})|(?:\\d{1,2})))|(?:[0-9A-Fa-f]{0,4}::(?:[0-9A-Fa-f]{0,4}:){0,5}[0-9A-Fa-f]{0,4})|(?:::(?:[0-9A-Fa-f]{0,4}:){0,6}[0-9A-Fa-f]{0,4})|(?:(?:[0-9A-Fa-f]{0,4}:){1,7}:))\\z";

        /// <summary>
        /// The method determines whether the passed address parameter is IPv4 or not
        /// </summary>
        /// <param name="address">
        /// Parameter address can be anything
        /// </param>
        /// <returns>
        /// The method returns whether the address is IPv4 protocol or not.
        /// </returns>
        public static bool IsIPv4Protocol(string address)
        {
            return Regex.IsMatch(address, IPv4_RFC3986, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// The method determines whether the passed address parameter is IPv6 or not
        /// </summary>
        /// <param name="address">
        /// Parameter address can be anything
        /// </param>
        /// <returns>
        /// The method returns whether the address is IPv6 protocol or not.
        /// </returns>
        public static bool IsIPv6Protocol(string address)
        {
            return Regex.IsMatch(address, IPv6_RFC2732_v2, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// The method determines the protocol based on the passed address parameter
        /// </summary>
        /// <param name="address">
        /// Parameter address can be anything
        /// </param>
        /// <returns>
        /// The method returns the protocol or "Invalid"
        /// </returns>
        public static Protocol DetectProtocol(string address)
        {
            if (Regex.IsMatch(address, IPv4_RFC3986, RegexOptions.IgnoreCase)) return Protocol.IPv4;
            else if (Regex.IsMatch(address, IPv6_RFC2732_v2, RegexOptions.IgnoreCase)) return Protocol.IPv6;
            else return Protocol.Invalid;
        }
    }
}