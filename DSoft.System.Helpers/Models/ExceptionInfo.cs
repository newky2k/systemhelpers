/*
 * This model was copied, initially, from an answer on stack overflow given by Lord of the Goo -> 
 * https://stackoverflow.com/a/72968664/20983759
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DSoft.System.Helpers.Models
{
    /// <summary>
    /// Exception Info
    /// </summary>
    public class ExceptionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfo"/> class.
        /// </summary>
        public ExceptionInfo() { }

        internal ExceptionInfo(Exception exception, bool includeInnerException = true, bool includeStackTrace = false)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Type = exception.GetType().FullName;
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = includeStackTrace ? exception.StackTrace : null;
            if (includeInnerException && exception.InnerException is not null)
            {
                InnerException = new ExceptionInfo(exception.InnerException, includeInnerException, includeStackTrace);
            }
        }

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
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the inner exception.
        /// </summary>
        /// <value>The inner exception.</value>
        public ExceptionInfo InnerException { get; set; }
    }
}
