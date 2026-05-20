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

    /// <summary>ObjCRuntime.Runtime.MarshalManagedException (iOS / Mac Catalyst)</summary>
    AppleMarshalManagedException,

    /// <summary>ObjCRuntime.Runtime.MarshalObjectiveCException (iOS / Mac Catalyst)</summary>
    AppleMarshalObjectiveCException,

    /// <summary>Microsoft.UI.Xaml.Application.Current.UnhandledException (Windows)</summary>
    WindowsXaml,
}
