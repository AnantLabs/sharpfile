namespace Common.Logger {
    /// <summary>
    /// Log level.
    /// </summary>
    public enum LogLevelType {
        /// <summary>
        /// Only log errors.
        /// </summary>
        ErrorsOnly = 0,
        /// <summary>
        /// Mildly verbose logging.
        /// Includes exception logs if available.
        /// </summary>
        MildlyVerbose = 1,
        /// <summary>
        /// Verbose logging.
        /// Includes traces and exception logs if available.
        /// </summary>
        Verbose = 2
    }
}