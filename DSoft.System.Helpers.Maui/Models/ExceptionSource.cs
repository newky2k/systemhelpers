namespace DSoft.System.Helpers.Maui.Models;

/// <summary>
/// Identifies which platform event handler captured the unhandled exception.
/// </summary>
public enum ExceptionSource
{
    /// <summary>AppDomain.CurrentDomain.UnhandledException</summary>
    AppDomain,

    /// <summary>TaskScheduler.UnobservedTaskException</summary>
    TaskScheduler,

    /// <summary>Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser</summary>
    Android,

    /// <summary>Java.Lang.Thread.DefaultUncaughtExceptionHandler</summary>
    AndroidJavaThread,

    /// <summary>Microsoft.UI.Xaml.Application.Current.UnhandledException (Windows)</summary>
    WindowsXaml,
}
