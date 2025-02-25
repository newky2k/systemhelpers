/*
 * This class was copied, initially, from an answer on github given by FreakyAli -> 
 * https://github.com/dotnet/maui/discussions/653#discussioncomment-11267333
 * 
 * Windows additions came from a gist by myrup
 * 
 * https://gist.github.com/myrup/43ee8038e0fd6ef4d31cbdd67449a997s
 * 
 */
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
        /// Occurs when [unhandled exception].
        /// </summary>
        public static event UnhandledExceptionEventHandler UnhandledException = delegate { };

        /// <summary>
        /// Initializes static members of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        static GlobalExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                UnhandledException?.Invoke(sender, args);
            };

            // Events fired by the TaskScheduler. That is calls like Task.Run(...)     
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, false));
            };

#if IOS || MACCATALYST

        // For iOS and Mac Catalyst
        // Exceptions will flow through AppDomain.CurrentDomain.UnhandledException,
        // but we need to set UnwindNativeCode to get it to work correctly. 
        // 
        // See: https://github.com/xamarin/xamarin-macios/issues/15252

        ObjCRuntime.Runtime.MarshalManagedException += (_, args) =>
        {
            args.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
        };

#elif ANDROID

            // For Android:
            // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
            // and NOT through AppDomain.CurrentDomain.UnhandledException

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (sender, args) =>
            {
                args.Handled = true;
                UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(args.Exception, true));
            };

            Java.Lang.Thread.DefaultUncaughtExceptionHandler = new CustomUncaughtExceptionHandler(e =>
                UnhandledException?.Invoke(null, new UnhandledExceptionEventArgs(e, true)));
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
        };
#endif
        }
    }

#if ANDROID
    public class CustomUncaughtExceptionHandler(Action<Java.Lang.Throwable> callback)
        : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
    {
        public void UncaughtException(Java.Lang.Thread t, Java.Lang.Throwable e)
        {
            callback(e);
        }
    }
#endif
}
