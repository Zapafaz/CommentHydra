using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CommentHydra
{
    /// <summary>
    /// Adds or updates header comments for every file in a folder.
    /// </summary>
    internal class Commenter
    {
        internal string[] NonCommentIndicators { get; }
        internal string[] NewComments { get; }
        internal string Folder { get; }
        internal string Extension { get; }

        /// <summary>
        /// Constructor for the commenter class, used to add/update comments to the start of every file in a given folder.
        /// </summary>
        /// <param name="folder">The folder containing the files to add/update comments to.</param>
        /// <param name="nonComment">The type of non-comments to look for at the start of a file, i.e. when to stop looking for comments.</param>
        /// <param name="extension">The file extension that should have comments added to them.</param>
        internal Commenter(string folder, string[] nonComment, string extension, string[] newComments)
        {
            Folder = folder;
            NonCommentIndicators = nonComment;
            Extension = extension;
            NewComments = newComments;
        }

        /// <summary>
        /// Add comments to every file in a folder, and possibly all files in its subfolders.
        /// </summary>
        /// <param name="searchOption">Only add comments to files in the current folder, or to files in all subfolders as well?</param>
        /// <param name="replaceOldComments">False = files with existing header comments will be ignored.</param>
        internal void AddCommentsToAllFiles(SearchOption searchOption, bool replaceOldComments)
        {
            var files = Directory.GetFiles(Folder, '*'+Extension, searchOption);
            foreach (string path in files)
            {
                List<string> lines = DetermineRewriteLines(path, replaceOldComments);
                if (lines == null)
                {
                    continue;
                }
                File.Delete(path);
                Debug.WriteLine("Adding comments to: " + path);
                File.WriteAllLines(path, lines);
            }
        }

        /// <summary>
        /// Get the lines to write for the given file based on this.NewComments and replace old comments if <paramref name="replaceOldComments"/> == true
        /// </summary>
        /// <param name="path">The file to remove comments from.</param>
        /// <param name="replaceOldComments">Whether or not to replace old comments in the file.</param>
        /// <returns>Returns a list of lines to write.</returns>
        private List<string> DetermineRewriteLines(string path, bool replaceOldComments)
        {
            if (!replaceOldComments && HasHeaderComments(path))
            {
                Debug.WriteLine("File already has comments, ignoring: " + path);
                return null;
            }

            var linesToKeep = File.ReadAllLines(path).ToList();

            if (replaceOldComments)
            {
                // This should make it remove everything until it runs into something in NonCommentIndicators
                while (linesToKeep.Count > 0)
                {
                    // Trim in case of unusual whitespace style
                    if (NonCommentIndicators.Any(s => linesToKeep[0].Trim().StartsWith(s)))
                    {
                        break;
                    }
                    else
                    {
                        linesToKeep.RemoveAt(0);
                    }
                }
            }

            linesToKeep.InsertRange(0, NewComments);

            return linesToKeep;
        }

        /// <summary>
        /// Checks if a given file has comments at the top of a file already.
        /// </summary>
        /// <param name="path">The file to check for header comments.</param>
        /// <returns>Returns true if the file already has comments at the top, false if not.</returns>
        private bool HasHeaderComments(string path)
        {
            var lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (NonCommentIndicators.Any(s => line.Trim().StartsWith(s)))
                {
                    break;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
