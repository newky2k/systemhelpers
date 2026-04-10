/*
 * This class was copied, initially, from an answer on github given by FreakyAli ->
 * https://github.com/dotnet/maui/discussions/653#discussioncomment-11267333
 *
 * Windows additions came from a gist by myrup
 *
 * https://gist.github.com/myrup/43ee8038e0fd6ef4d31cbdd67449a997s
 *
 */

using DSoft.System.Helpers.Models;
using DSoft.System.Helpers.Maui.Models;

namespace DSoft.System.Helpers.Maui
{
    /// <summary>
    /// Global Exception Handler with MAUI specifics
    /// </summary>
    public static class GlobalExceptionHandler
    {

#if WINDOWS
        private static Exception _lastFirstChanceException;
#endif

        /// <summary>
        /// Occurs when an unhandled exception is raised. Provides the raw
        /// <see cref="UnhandledExceptionEventArgs"/> for backwards compatibility.
        /// </summary>
        public static event UnhandledExceptionEventHandler UnhandledException = delegate { };

        /// <summary>
        /// Occurs when an unhandled exception is raised. Provides a rich
        /// <see cref="UnhandledExceptionReport"/> with exception details, app info,
        /// and device info similar to AppCenter / Sentry crash reports.
        /// </summary>
        public static event EventHandler<UnhandledExceptionReportEventArgs> UnhandledExceptionOccurred = delegate { };

        /// <summary>
        /// Initializes static members of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        static GlobalExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                UnhandledException?.Invoke(sender, args);

                if (args.ExceptionObject is Exception ex)
                    FireReport(ex, ExceptionSource.AppDomain, args.IsTerminating);
            };

            // Events fired by the TaskScheduler. That is calls like Task.Run(...)
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, false));
                FireReport(args.Exception, ExceptionSource.TaskScheduler, false);
            };

#if IOS || MACCATALYST

        // For iOS and Mac Catalyst
        // Exceptions will flow through AppDomain.CurrentDomain.UnhandledException,
        // but we need to set UnwindNativeCode to get it to work correctly.
        //
        // See: https://github.com/xamarin/xamarin-macios/issues/15252

            ObjCRuntime.Runtime.MarshalManagedException += (_, e) => e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
            ObjCRuntime.Runtime.MarshalObjectiveCException += (_, e) => e.ExceptionMode = ObjCRuntime.MarshalObjectiveCExceptionMode.UnwindManagedCode;

#elif ANDROID

            // For Android:
            // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
            // and NOT through AppDomain.CurrentDomain.UnhandledException

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                args.Handled = true;
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, true));
                FireReport(args.Exception, ExceptionSource.Android, true);
            };

            Java.Lang.Thread.DefaultUncaughtExceptionHandler = new CustomUncaughtExceptionHandler((t, e) =>
            {
                UnhandledException?.Invoke(null, new UnhandledExceptionEventArgs(e, true));
                FireReportFromThrowable(e, ExceptionSource.AndroidJavaThread, true);
            });
#elif WINDOWS

        // For WinUI 3:
        //
        // * Exceptions on background threads are caught by AppDomain.CurrentDomain.UnhandledException,
        //   not by Microsoft.UI.Xaml.Application.Current.UnhandledException
        //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/5221
        //
        // * Exceptions caught by Microsoft.UI.Xaml.Application.Current.UnhandledException have details removed,
        //   but that can be worked around by saved by trapping first chance exceptions
        //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/7160
        //

        AppDomain.CurrentDomain.FirstChanceException += (_, args) =>
        {
            _lastFirstChanceException = args.Exception;
        };

        Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, args) =>
        {
            var exception = args.Exception;

            if (exception.StackTrace is null)
            {
                exception = _lastFirstChanceException;
            }

            UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(exception, true));
            FireReport(exception, ExceptionSource.WindowsXaml, true);
        };
#endif
        }

        private static void FireReport(Exception exception, ExceptionSource source, bool isTerminating)
        {
            if (UnhandledExceptionOccurred is null)
                return;

            var info = new ExceptionInfo(exception, includeInnerException: true, includeStackTrace: true);
            var report = new UnhandledExceptionReport
            {
                Exception = info,
                Source = source,
                IsTerminating = isTerminating,
            };

            PopulateContext(report);
            UnhandledExceptionOccurred.Invoke(null, new UnhandledExceptionReportEventArgs(report));
        }

#if ANDROID
        private static void FireReportFromThrowable(Java.Lang.Throwable throwable, ExceptionSource source, bool isTerminating)
        {
            if (UnhandledExceptionOccurred is null)
                return;

            var currentThread = Thread.CurrentThread;
            var info = new ExceptionInfo
            {
                Timestamp = DateTimeOffset.UtcNow,
                Type = throwable.GetType().FullName,
                Message = throwable.Message,
                StackTrace = throwable.StackTrace,
                ThreadId = currentThread.ManagedThreadId,
                ThreadName = currentThread.Name,
            };

            var report = new UnhandledExceptionReport
            {
                Exception = info,
                Source = source,
                IsTerminating = isTerminating,
            };

            PopulateContext(report);
            UnhandledExceptionOccurred.Invoke(null, new UnhandledExceptionReportEventArgs(report));
        }
#endif

        private static void PopulateContext(UnhandledExceptionReport report)
        {
            try
            {
                report.AppId = AppInfo.Current.PackageName;
                report.AppName = AppInfo.Current.Name;
                report.AppVersion = AppInfo.Current.Version.ToString();
                report.AppBuild = AppInfo.Current.BuildString;
            }
            catch { }

            try
            {
                report.Platform = DeviceInfo.Current.Platform.ToString();
                report.OSVersion = DeviceInfo.Current.VersionString;
                report.DeviceModel = DeviceInfo.Current.Model;
                report.DeviceManufacturer = DeviceInfo.Current.Manufacturer;
                report.DeviceType = DeviceInfo.Current.DeviceType.ToString();
                report.DeviceIdiom = DeviceInfo.Current.Idiom.ToString();
                report.DeviceName = DeviceInfo.Current.Name;
            }
            catch { }
        }
    }

#if ANDROID
    public class CustomUncaughtExceptionHandler(Action<Java.Lang.Thread, Java.Lang.Throwable> callback)
        : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
    {
        public void UncaughtException(Java.Lang.Thread t, Java.Lang.Throwable e)
        {
            callback(t, e);
        }
    }
#endif
}
