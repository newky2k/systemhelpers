/*
 * This extensions was copied, initially, from an answer on stack overflow given by Lord of the Goo -> 
 * https://stackoverflow.com/a/72968664/20983759
 * 
 */
using DSoft.System.Helpers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace System
{
    /// <summary>
    /// Extensions for Exception
    /// </summary>
    public static class ExceptionExtensions
    {
        private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        /// <summary>
        /// Serialize the <see cref="Exception"/> to a JSON string.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="includeInnerException">Control if to include inner exception</param>
        /// <param name="includeStackTrace">Control if to include stack trace</param>
        /// <param name="options">JSON options. By default nulls are not serialized and the string is indented</param>
        /// <returns></returns>
        public static string ToJson(
            this Exception ex,
            bool includeInnerException = true,
            bool includeStackTrace = false,
            JsonSerializerOptions options = null)
        {

#if NETSTANDARD 
            if (ex is null)
                throw new ArgumentNullException("ex");
#else
            ArgumentNullException.ThrowIfNull(ex);
#endif
            var info = new ExceptionInfo(ex, includeInnerException, includeStackTrace);

            return JsonSerializer.Serialize(info, options ?? _defaultJsonSerializerOptions);
        }

        /// <summary>
        /// Serialize the <see cref="Exception"/> to a JSON byte array.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="includeInnerException">Control if to include inner exception</param>
        /// <param name="includeStackTrace">Control if to include stack trace</param>
        /// <param name="options">JSON options. By default nulls are not serialized and the string is indented</param>
        /// <returns></returns>
        public static byte[] ToJsonBytes(
            this Exception ex,
            bool includeInnerException = true,
            bool includeStackTrace = false,
            JsonSerializerOptions options = null)
        {
            var jsonString = ex.ToJson(includeInnerException, includeStackTrace, options);

            return jsonString.ToBase64();
        }
    }
}
