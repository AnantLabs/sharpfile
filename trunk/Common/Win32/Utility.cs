using System;

namespace Common.Win32 {
    public static class Utility {
        /// <summary>
        /// Converts the FILETIME structure to the correct DateTime.
        /// </summary>
        /// <param name="fileTime">FILETIME to convert.</param>
        /// <returns>DateTime.</returns>
        /// <remarks>This is not in a common shared library because it relies on SharpFile.Infrastructure.</remarks>
        public static DateTime ConvertFileTimeToDateTime(FILETIME fileTime) {
            long ticks = (((long)fileTime.dwHighDateTime) << 32) + (long)fileTime.dwLowDateTime;
            DateTime dateTime = DateTime.FromFileTime(ticks);

            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(dateTime) == false) {
                dateTime = dateTime.AddHours(1);
            }

            return dateTime;
        }
    }
}