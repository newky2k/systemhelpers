using DSoft.System.Helpers.Models;

namespace DSoft.System.Helpers.Maui.Models;

/// <summary>
/// A rich crash report captured by <see cref="GlobalExceptionHandler"/>, combining exception
/// details with app and device context similar to AppCenter / Sentry crash reports.
/// </summary>
public class UnhandledExceptionReport
{
    // ── Exception ─────────────────────────────────────────────────────────

    /// <summary>Full exception details including timestamp, stack trace, and thread info.</summary>
    public ExceptionInfo Exception { get; set; }

    /// <summary>The platform event handler that captured the exception.</summary>
    public ExceptionSource Source { get; set; }

    /// <summary>Whether the exception is causing the application to terminate.</summary>
    public bool IsTerminating { get; set; }

    // ── App ───────────────────────────────────────────────────────────────

    /// <summary>Application package / bundle identifier.</summary>
    public string AppId { get; set; }

    /// <summary>Application display name.</summary>
    public string AppName { get; set; }

    /// <summary>Application version (e.g. "1.2.3").</summary>
    public string AppVersion { get; set; }

    /// <summary>Application build number.</summary>
    public string AppBuild { get; set; }

    // ── Device / OS ───────────────────────────────────────────────────────

    /// <summary>Operating system platform (e.g. "Android", "iOS", "WinUI").</summary>
    public string Platform { get; set; }

    /// <summary>Operating system version string.</summary>
    public string OSVersion { get; set; }

    /// <summary>Device model name.</summary>
    public string DeviceModel { get; set; }

    /// <summary>Device manufacturer.</summary>
    public string DeviceManufacturer { get; set; }

    /// <summary>Device type: Physical or Virtual.</summary>
    public string DeviceType { get; set; }

    /// <summary>Device form-factor idiom (Phone, Tablet, Desktop, etc.).</summary>
    public string DeviceIdiom { get; set; }

    /// <summary>Device name as reported by the OS.</summary>
    public string DeviceName { get; set; }
}
