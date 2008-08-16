namespace Common {
    public static class Path {
        public static string DirectorySeparator = System.IO.Path.DirectorySeparatorChar.ToString();
        public static string AltDirectorySeparator = System.IO.Path.AltDirectorySeparatorChar.ToString();
        public static string VolumeSeparator = System.IO.Path.VolumeSeparatorChar.ToString();

        /// <summary>
        /// Clean up path trying to correct obvious mis-typings.
        /// </summary>
        /// <param name="path">Path to clean up.</param>
        /// <returns>Cleaned up path.</returns>
        public static string Cleanup(string path) {
            if (string.IsNullOrEmpty(path)) {
                return path;
            }

            string cleanPath = path;

            // Fix any wayward slashes.
            if (cleanPath.EndsWith(AltDirectorySeparator)) {
                cleanPath = cleanPath.Substring(0, cleanPath.Length - 1);
            }

            cleanPath = cleanPath.Replace(AltDirectorySeparator, DirectorySeparator);

            if (!string.IsNullOrEmpty(cleanPath)) {
                if (cleanPath.EndsWith(VolumeSeparator)) {
                    cleanPath = string.Format("{0}{1}",
                        cleanPath,
                        DirectorySeparator);
                } else if (!cleanPath.Contains(VolumeSeparator + DirectorySeparator)) {
                    cleanPath = string.Format("{0}{1}{2}",
                        cleanPath,
                        VolumeSeparator,
                        DirectorySeparator);
                } else if (path.EndsWith(AltDirectorySeparator)) {
                    cleanPath = cleanPath.Replace(AltDirectorySeparator, DirectorySeparator);
                }

                // Remove any duplicated slashes.
                cleanPath = cleanPath.Replace(DirectorySeparator + DirectorySeparator, DirectorySeparator);
            }

            if (!cleanPath.EndsWith(DirectorySeparator) && System.IO.Directory.Exists(cleanPath)) {
                cleanPath = string.Format(@"{0}{1}",
                        cleanPath,
                        DirectorySeparator);
            }

            return cleanPath;
        }
    }
}