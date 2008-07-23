using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Logger {
    public class LoggerService {
        string fileName;
        LogLevelType _logLevel = LogLevelType.ErrorsOnly;
        Logger logger;

        public delegate void ProcessContentDelegate(string content);
        public event ProcessContentDelegate ProcessContent;

        /// <summary>
        /// Default ctor.
        /// Default LogLevel is ErrorOnly.
        /// </summary>
        /// <param name="fileName">Filename to log to.</param>
        public LoggerService(string fileName)
            : this(fileName, LogLevelType.ErrorsOnly) {
        }

        /// <summary>
        /// Regular ctor.
        /// </summary>
        /// <param name="fileName">Filename to log to.</param>
        /// <param name="logLevel">LogLevel to log as.</param>
        public LoggerService(string fileName, LogLevelType logLevel) {
            this.fileName = fileName;
            _logLevel = logLevel;

            switch (logLevel) {
                case LogLevelType.ErrorsOnly:
                    logger = new ErrorsOnlyLogger();
                    break;
                case LogLevelType.MildlyVerbose:
                    logger = new MildlyVerboseLogger();
                    break;
                case LogLevelType.Verbose:
                    logger = new VerboseLogger();
                    break;
                default:
                    logger = new ErrorsOnlyLogger();
                    break;
            }
        }

        /// <summary>
        /// Raises the ProcessContent event.
        /// </summary>
        /// <param name="content">Content to raise.</param>
        /// <param name="arguments">Arguments to the content.</param>
        public void OnProcessContent(string content, string[] arguments) {
            if (ProcessContent != null) {
                ProcessContent(string.Format(content,
                    arguments));
            }
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="content">Content to log.</param>
        public void Log(LogLevelType logLevel, string content) {
            Log(logLevel, null, content, new string[] {});
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public void Log(LogLevelType logLevel, string content, params string[] arguments) {
            Log(logLevel, null, content, arguments);
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="content">Content to log.</param>
        public void Log(LogLevelType logLevel, Exception exception, string content) {
            Log(logLevel, exception, content, new string[] {});
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public void Log(LogLevelType logLevel, Exception exception, string content, params string[] arguments) {
            if (LogLevel >= logLevel) {
                try {
                    OnProcessContent(content, arguments);
                } catch (Exception ex) {
                    logger.Log(fileName, ex, "Error while trying to process the content.", new string[] { });
                }

                logger.Log(fileName, exception, content, arguments);
            }
        }

        /// <summary>
        /// Filename of the log.
        /// </summary>
        public string FileName {
            get {
                return fileName;
            }
        }

        /// <summary>
        /// LogLevel.
        /// </summary>
        public LogLevelType LogLevel {
            get {
                return _logLevel;
            }
        }
    }
}