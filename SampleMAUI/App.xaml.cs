using DSoft.System.Helpers.Maui;
using DSoft.System.Helpers.Maui.Models;

namespace SampleMAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        GlobalExceptionHandler.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
    }

    private void OnUnhandledExceptionOccurred(object? sender, UnhandledExceptionReportEventArgs e)
    {
        var report = e.Report;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var page = Windows.FirstOrDefault()?.Page;

            if (page is not null)
            {
                await page.DisplayAlertAsync(
                    "Unhandled Exception Caught",
                    $"Source:  {report.Source}\n" +
                    $"Type:    {report.Exception?.Type}\n" +
                    $"Message: {report.Exception?.Message}\n" +
                    $"App:     {report.AppName} {report.AppVersion}\n" +
                    $"Device:  {report.DeviceModel} ({report.Platform} {report.OSVersion})\n" +
                    $"Terminating: {report.IsTerminating}",
                    "OK");
            }
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}