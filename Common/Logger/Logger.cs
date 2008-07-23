using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Common.Logger {
    /// <summary>
    /// Internal logger class that is the base for all of the other logger classes.
    /// </summary>
    internal abstract class Logger {
        private static object lockObject = new object();

        /// <summary>
        /// Helper method to open the log file and write to it.
        /// </summary>
        /// <param name="content">Content to write to the file.</param>
        /// <param name="logLevel">The log level that will write the content.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public void Log(string fileName, Exception exception, string content, params string[] arguments) {
            lock (lockObject) {
                // Append to our log file if we are the level specified or above.
                using (StreamWriter sw = new StreamWriter(fileName, true)) {
                    content = string.Format(content,
                        arguments);

                    content = getMessage(exception, content);

                    sw.WriteLine(content);
                    sw.Flush();
                }
            }
        }

        /// <summary>
        /// Gets the message that should be logged.
        /// </summary>
        /// <param name="exception">Exception to add to the message if necessary.</param>
        /// <param name="content">Content for the message.</param>
        /// <returns>Message.</returns>
        protected abstract string getMessage(Exception exception, string content);

        /// <summary>
        /// Retrieves a string representation of the inner exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns>Representation of the exception.</returns>
        protected string getExceptionMessage(Exception exception) {
            string exceptionMessage = string.Empty;

            if (exception != null) {
                Exception innerException = General.GetInnerException(exception);

                exceptionMessage = string.Format("{0}: {1}: ",
                    innerException.Message,
                    innerException.StackTrace);
            }

            return exceptionMessage;
        }
    }
}