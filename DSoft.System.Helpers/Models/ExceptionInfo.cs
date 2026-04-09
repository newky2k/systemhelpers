/*
 * This model was copied, initially, from an answer on stack overflow given by Lord of the Goo ->
 * https://stackoverflow.com/a/72968664/20983759
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DSoft.System.Helpers.Models;

/// <summary>
/// Exception Info
/// </summary>
public class ExceptionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
    /// </summary>
    public ExceptionInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionInfo"/> class from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception to capture.</param>
    /// <param name="includeInnerException">Whether to recursively capture inner exceptions.</param>
    /// <param name="includeStackTrace">Whether to include the stack trace.</param>
    public ExceptionInfo(Exception exception, bool includeInnerException = true, bool includeStackTrace = false)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        Timestamp = DateTimeOffset.UtcNow;
        Type = exception.GetType().FullName;
        Message = exception.Message;
        Source = exception.Source;
        HResult = exception.HResult;
        HelpLink = exception.HelpLink;
        StackTrace = includeStackTrace ? exception.StackTrace : null;

        var currentThread = Thread.CurrentThread;
        ThreadId = currentThread.ManagedThreadId;
        ThreadName = currentThread.Name;

        if (exception.Data is { Count: > 0 })
        {
            var data = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in exception.Data)
            {
                data[entry.Key?.ToString() ?? string.Empty] = entry.Value?.ToString();
            }
            Data = data;
        }

        if (includeInnerException && exception.InnerException is not null)
        {
            InnerException = new ExceptionInfo(exception.InnerException, true, includeStackTrace);
        }
    }

    /// <summary>
    /// Gets or sets the UTC timestamp when the exception was captured.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the source assembly or object that caused the exception.
    /// </summary>
    /// <value>The source.</value>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the HRESULT error code associated with the exception.
    /// </summary>
    public int HResult { get; set; }

    /// <summary>
    /// Gets or sets a link to a help file associated with the exception.
    /// </summary>
    public string HelpLink { get; set; }

    /// <summary>
    /// Gets or sets the stack trace.
    /// </summary>
    /// <value>The stack trace.</value>
    public string StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the managed thread ID on which the exception was thrown.
    /// </summary>
    public int ThreadId { get; set; }

    /// <summary>
    /// Gets or sets the name of the thread on which the exception was thrown.
    /// </summary>
    public string ThreadName { get; set; }

    /// <summary>
    /// Gets or sets key/value pairs from <see cref="Exception.Data"/>, if any.
    /// </summary>
    public Dictionary<string, string> Data { get; set; }

    /// <summary>
    /// Gets or sets the inner exception.
    /// </summary>
    /// <value>The inner exception.</value>
    public ExceptionInfo InnerException { get; set; }
}
