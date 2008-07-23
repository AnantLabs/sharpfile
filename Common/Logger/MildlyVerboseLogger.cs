using System;

namespace Common.Logger {
    internal class MildlyVerboseLogger : Logger {
        protected override string getMessage(Exception exception, string content) {
            string exceptionMessage = getExceptionMessage(exception);

            content = string.Format("{0}: {1}{2}",
                DateTime.Now,
                string.IsNullOrEmpty(exceptionMessage) ? string.Empty : exceptionMessage,
                content);

            return content;
        }
    }
}