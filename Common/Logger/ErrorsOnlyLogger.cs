using System;

namespace Common.Logger {
    internal class ErrorsOnlyLogger : Logger {
        protected override string getMessage(Exception exception, string content) {
            return string.Format("{0}: {1}",
                            DateTime.Now,
                            content);
        }
    }
}