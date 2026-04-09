namespace SampleMAUI;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private async void OnThrowExceptionClicked(object? sender, EventArgs e)
    {
        // Fire a Task that throws but don't observe it. It flows through
        // TaskScheduler.UnobservedTaskException → GlobalExceptionHandler,
        // which is non-terminating so the alert can display on all platforms.
        Task.Run(() => throw new InvalidOperationException("Demo unhandled exception via TaskScheduler"));

        // Wait for the task to reach a faulted state, then nudge the GC so
        // UnobservedTaskException fires promptly rather than at a random GC cycle.
        await Task.Delay(100);
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}