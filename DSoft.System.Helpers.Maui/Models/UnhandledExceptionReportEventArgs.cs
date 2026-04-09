namespace DSoft.System.Helpers.Maui.Models;

/// <summary>
/// Event args for <see cref="GlobalExceptionHandler.UnhandledExceptionOccurred"/>.
/// </summary>
public class UnhandledExceptionReportEventArgs : EventArgs
{
    /// <summary>
    /// The rich crash report containing exception details, app info, and device info.
    /// </summary>
    public UnhandledExceptionReport Report { get; }

    /// <summary>Initializes a new instance of <see cref="UnhandledExceptionReportEventArgs"/>.</summary>
    public UnhandledExceptionReportEventArgs(UnhandledExceptionReport report)
    {
        Report = report;
    }
}
