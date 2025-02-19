using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// String Extensions.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Converts the string to a base64 byte array.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns>System.String.</returns>
        public static byte[] ToBase64(this string plainText, bool reverse = false)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            if (reverse)
            {
                plainTextBytes = plainTextBytes.Reverse().ToArray();
            }


            return plainTextBytes;
        }

        /// <summary>
        /// Converts the string to a base64 string.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns>System.String.</returns>
        public static string ToBase64String(this string plainText, bool reverse = false)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            if (reverse)
            {
                plainTextBytes = plainTextBytes.Reverse().ToArray();
            }

            
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode the base64 data into a string
        /// </summary>
        /// <param name="base64EncodedData">The base64 encoded data.</param>
        /// <param name="reversed">if set to <c>true</c> [reversed].</param>
        /// <returns>System.String.</returns>
        public static string Base64Decode(this string base64EncodedData, bool reversed = false)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

            var result = Encoding.UTF8.GetString(base64EncodedBytes);

            if (!reversed)
            {
                return result;
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(result);
            plainTextBytes = plainTextBytes.Reverse().ToArray();

            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }
}
