using DSoft.System.Helpers.Maui.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DSoft.System.Helpers.Maui.Extensions;

/// <summary>
/// Extensions for <see cref="UnhandledExceptionReport"/>.
/// </summary>
public static class UnhandledExceptionReportExtensions
{
    private static readonly JsonSerializerOptions _defaultJsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    /// <summary>
    /// Serializes the <see cref="UnhandledExceptionReport"/> to a JSON string.
    /// </summary>
    /// <param name="report">The report to serialize.</param>
    /// <param name="options">JSON options. By default nulls are not serialized and the output is indented.</param>
    /// <returns>A JSON string representation of the report.</returns>
    public static string ToJson(this UnhandledExceptionReport report, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(report);

        return JsonSerializer.Serialize(report, options ?? _defaultJsonSerializerOptions);
    }

    /// <summary>
    /// Serializes the <see cref="UnhandledExceptionReport"/> to a UTF-8 byte array of the JSON string.
    /// </summary>
    /// <param name="report">The report to serialize.</param>
    /// <param name="options">JSON options. By default nulls are not serialized and the output is indented.</param>
    /// <returns>A UTF-8 encoded byte array of the JSON representation of the report.</returns>
    public static byte[] ToJsonBytes(this UnhandledExceptionReport report, JsonSerializerOptions? options = null)
    {
        var json = report.ToJson(options);

        return Encoding.UTF8.GetBytes(json);
    }
}
