namespace TAUtil.Hpi
{
    using System;

    /// <summary>
    /// Custom implementation of various Path functions
    /// for working with HPI paths,
    /// which can contain "invalid" characters.
    /// </summary>
    public static class HpiPath
    {
        public static readonly char DirectorySeparatorChar = '\\';

        public static readonly char AltDirectorySeparatorChar = '/';

        /// <summary>
        /// Returns the extension of the specified HPI path string.
        /// </summary>
        ///
        /// <param name="path">
        /// The HPI path string from which to get the extension.
        /// </param>
        ///
        /// <returns>
        /// The extension of the specified HPI path (including the period "."),
        /// or null, or string.Empty.
        /// If path is null, returns null.
        /// If path does not have extension information,
        /// returns string.Empty.
        /// </returns>
        public static string GetExtension(string path)
        {
            if (path == null)
            {
                return null;
            }

            int length = path.Length;
            int startIndex = length;
            while (--startIndex >= 0)
            {
                char ch = path[startIndex];
                if (ch == '.')
                {
                    if (startIndex == length - 1)
                    {
                        return string.Empty;
                    }

                    return path.Substring(startIndex, length - startIndex);
                }

                if (ch == DirectorySeparatorChar
                    || ch == AltDirectorySeparatorChar)
                {
                    break;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the file name of the specified HPI path string
        /// without the extension.
        /// </summary>
        /// <param name="path">The HPI path of the file.</param>
        /// <returns>
        /// The string returned by GetFileName,
        /// minus the last period (.) and all characters following it.
        /// </returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            path = HpiPath.GetFileName(path);
            if (path == null)
            {
                return null;
            }

            int length = path.LastIndexOf('.');
            if (length == -1)
            {
                return path;
            }

            return path.Substring(0, length);
        }

        /// <summary>
        /// Returns the file name and extension of the specified HPI path string.
        /// </summary>
        /// <param name="path">
        /// The HPI path string from which to obtain the file name and extension.
        /// </param>
        /// <returns>
        /// The characters after the last directory character in path.
        /// If the last character of path is a directory separator character,
        /// this method returns string.Empty.
        /// If path is null, this method returns null.
        /// </returns>
        public static string GetFileName(string path)
        {
            if (path == null)
            {
                return null;
            }

            int length = path.Length;
            int index = length;
            while (--index >= 0)
            {
                char ch = path[index];
                if (ch == DirectorySeparatorChar
                    || ch == AltDirectorySeparatorChar)
                {
                    return path.Substring(index + 1, length - index - 1);
                }
            }

            return path;
        }

        /// <param name="path">
        /// The path of a file or directory in an HPI file.
        /// </param>
        /// <returns>
        /// Directory information for path, or null if path is null.
        /// Returns string.Empty if path does not contain directory information.
        /// </returns>
        public static string GetDirectoryName(string path)
        {
            if (path == null)
            {
                return null;
            }

            int length = path.Length;
            int index = length;
            while (--index >= 0)
            {
                char ch = path[index];
                if (ch == DirectorySeparatorChar
                    || ch == AltDirectorySeparatorChar)
                {
                    return path.Substring(0, index);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Combines two strings into an HPI path.
        /// </summary>
        /// <param name="path1">The first HPI path to combine.</param>
        /// <param name="path2">The second HPI path to combine.</param>
        /// <returns>
        /// The combined paths.
        /// If one of the specified paths is a zero-length string,
        /// this method returns the other path.
        /// </returns>
        public static string Combine(string path1, string path2)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException("path1");
            }

            if (path2 == null)
            {
                throw new ArgumentNullException("path2");
            }

            if (path1.Length == 0)
            {
                return path2;
            }

            if (path2.Length == 0)
            {
                return path1;
            }

            char ch = path1[path1.Length - 1];
            if (ch == DirectorySeparatorChar
                || ch == AltDirectorySeparatorChar)
            {
                return path1 + path2;
            }

            return path1 + DirectorySeparatorChar + path2;
        }

        /// <summary>
        /// Changes the extension of a HPI path string.
        /// </summary>
        /// <param name="path">The HPI path information to modify.</param>
        /// <param name="extension">
        /// The new extension (with or without a leading period).
        /// Specify null to remove an existing extension from path. 
        /// </param>
        /// <returns>
        /// if path is null or an empty string (""),
        /// the path information is returned unmodified.
        /// If extension is null,
        /// the returned string contains the specified path
        /// with its extension removed.
        /// If path has no extension, and extension is not null,
        /// the returned path string contains extension
        /// appended to the end of path.
        /// </returns>
        public static string ChangeExtension(string path, string extension)
        {
            if (path == null)
            {
                return null;
            }

            string str = path;
            int length = path.Length;
            while (--length >= 0)
            {
                char ch = path[length];
                if (ch == '.')
                {
                    str = path.Substring(0, length);
                    break;
                }

                if (ch == DirectorySeparatorChar
                    || ch == AltDirectorySeparatorChar)
                {
                    break;
                }
            }

            if (extension != null && path.Length != 0)
            {
                if (extension.Length == 0 || extension[0] != '.')
                {
                    str = str + ".";
                }

                str = str + extension;
            }

            return str;
        }
    }
}
