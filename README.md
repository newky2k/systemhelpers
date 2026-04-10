# DSoft.System.Helpers

General helpers for C# and Modern .NET as well as OG .NET Framework 4.8(through .NET Standard 2.1).  

This is a compilation of some of the the code I've created and used over the years and some public contributions on public forums and web sites across the internet(detailed below).


## Features

### DSoft.System.Helpers

 - Object mapper
 - Embedded Resource Helper
 - Exception to JSON extension
 - Base64 encode/decode string extensions

### DSoft.System.Helpers.Maui

 - Global exception handler with MAUI awareness and rich crash reporting


## DSoft.System.Helpers.Maui

### GlobalExceptionHandler

`GlobalExceptionHandler` is a static class that hooks all unhandled exception sources across Android, iOS, Mac Catalyst, and Windows in a single call. It exposes two events:

| Event | Args | Use |
| --- | --- | --- |
| `UnhandledException` | `UnhandledExceptionEventArgs` | Drop-in replacement for `AppDomain.CurrentDomain.UnhandledException` |
| `UnhandledExceptionOccurred` | `UnhandledExceptionReportEventArgs` | Rich crash report with exception details, app info, and device info |

Subscribe to either event early in app startup (e.g. in `App()`) to activate the handler:

```csharp
public App()
{
    InitializeComponent();

    GlobalExceptionHandler.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
}

private void OnUnhandledExceptionOccurred(object? sender, UnhandledExceptionReportEventArgs e)
{
    var report = e.Report;
    // log, upload, or display report.Exception, report.AppVersion, report.DeviceModel, etc.
}
```

#### Platform hooks installed automatically

| Platform | Hook |
| --- | --- |
| All | `AppDomain.CurrentDomain.UnhandledException` |
| All | `TaskScheduler.UnobservedTaskException` |
| Android | `AndroidEnvironment.UnhandledExceptionRaiser` |
| Android | `Java.Lang.Thread.DefaultUncaughtExceptionHandler` |
| iOS / Mac Catalyst | `ObjCRuntime.Runtime.MarshalManagedException` |
| Windows | `Microsoft.UI.Xaml.Application.Current.UnhandledException` + first-chance exception workaround |

#### UnhandledExceptionReport

The `UnhandledExceptionReport` passed via `UnhandledExceptionReportEventArgs.Report` contains:

```
Exception         ExceptionInfo    Type, message, stack trace, thread, timestamp, inner exception
Source            ExceptionSource  Which platform hook captured the exception
IsTerminating     bool             Whether the process is about to terminate

AppId             string           Package/bundle identifier
AppName           string           Application display name
AppVersion        string           Version string
AppBuild          string           Build number

Platform          string           Android / iOS / WinUI / etc.
OSVersion         string           OS version string
DeviceModel       string           Device model name
DeviceManufacturer string          Device manufacturer
DeviceType        string           Physical or Virtual
DeviceIdiom       string           Phone / Tablet / Desktop / etc.
DeviceName        string           Device name as reported by the OS
```

`ExceptionSource` identifies which hook fired: `AppDomain`, `TaskScheduler`, `Android`, `AndroidJavaThread`, or `WindowsXaml`.

---

## Credits

This library uses public contributions from the following people and places.

 - Lord of the Goo
	- https://stackoverflow.com/a/72968664/20983759
 - FreakyAli
	- https://github.com/dotnet/maui/discussions/653#discussioncomment-11267333
 - myrup
	- https://gist.github.com/myrup/43ee8038e0fd6ef4d31cbdd67449a997s
