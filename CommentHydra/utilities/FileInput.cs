using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommentHydra.Utilities
{
    /// <summary>
    /// File input handling.
    /// </summary>
    class FileInput
    {
        /// <summary>
        /// Get all files in a given directory that match a given set of extensions.
        /// </summary>
        /// <param name="dir">The directory to get files from.</param>
        /// <param name="includeSubfolders">Whether or not to get files from subfolders as well.</param>
        /// <param name="extensions">The list of extensions to return, if any.</param>
        /// <returns>Returns the set of all files from the given folder, and subfolders, if includeSubfolders.</returns>
        internal static string[] GetFiles(string dir, bool includeSubfolders, string[] extensions)
        {
            try
            {
                var files = Directory.GetFiles(dir).ToList();

                if (includeSubfolders)
                {
                    foreach (string folder in GetAllFolders(dir, includeSubfolders))
                    {
                        foreach (string extension in extensions)
                        {
                            files.AddRange(Directory.GetFiles(folder, "*" + extension));
                        }
                    }
                }

                return files.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Program.CloseProgram();
                throw;
            }
        }

        /// <summary>
        /// Get all folders from a given directory.
        /// </summary>
        /// <param name="dir">The directory to get folders from.</param>
        /// <param name="includeSubfolders">Whether or not to get subfolders as well.</param>
        /// <returns>Returns all of the folders in the given folder, and all subfolders, if includeSubfolders.</returns>
        internal static string[] GetAllFolders(string dir, bool includeSubfolders)
        {
            var folders = Directory.GetDirectories(dir).ToList();

            if (includeSubfolders)
            {
                foreach (string path in folders)
                {
                    folders.AddRange(Directory.GetDirectories(path));
                }
            }
            return folders.ToArray();
        }
    }
}
