using System;

namespace Common.Logger {
    public class LoggerService {
        public delegate void ProcessContentDelegate(string content);

        private string fileName;
        private LogLevelType _logLevel = LogLevelType.ErrorsOnly;
        private Logger logger;        

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
        public bool Log(LogLevelType logLevel, string content, params string[] arguments) {
            return Log(logLevel, null, content, arguments);
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="content">Content to log.</param>
        public bool Log(LogLevelType logLevel, Exception exception, string content) {
            return Log(logLevel, exception, content, new string[] {});
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public bool Log(LogLevelType logLevel, Exception exception, string content, params string[] arguments) {
            bool logged = false;

            if (LogLevel >= logLevel) {
                logger.Log(fileName, exception, content, arguments);
                logged = true;
            }

            return logged;
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="method">ProcessMethodDelegate to invoke.</param>
        /// <param name="content">Content to log.</param>
        public bool LogAndInvoke(LogLevelType logLevel, ProcessContentDelegate method, string content) {
            return LogAndInvoke(logLevel, method, null, content, new string[] {});
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="method">ProcessMethodDelegate to invoke.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public bool LogAndInvoke(LogLevelType logLevel, ProcessContentDelegate method, string content, 
            params string[] arguments) {
            return LogAndInvoke(logLevel, method, null, content, arguments);
        }

        /// <summary>
        /// Log the message to the configured log file.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="method">ProcessMethodDelegate to invoke.</param>
        /// <param name="exception">Exception to log.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public bool LogAndInvoke(LogLevelType logLevel, ProcessContentDelegate method, Exception exception,
            string content, params string[] arguments) {
            bool logged = false;
            logged = Log(logLevel, exception, content, arguments);

            if (logged) {
                if (method != null) {
                    method.DynamicInvoke(string.Format(content, arguments));
                }
            }

            return logged;
        }

        /// <summary>
        /// Logs numerous exceptions with the same content.
        /// </summary>
        /// <param name="logLevel">LogLevel.</param>
        /// <param name="exceptions">Exceptions to log.</param>
        /// <param name="content">Content to log.</param>
        /// <param name="arguments">Arguments for the content.</param>
        public void LogNumerous(LogLevelType logLevel, Exception[] exceptions, string content, params string[] arguments) {
            foreach (Exception exception in exceptions) {
                Log(logLevel, exception, content, arguments);
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