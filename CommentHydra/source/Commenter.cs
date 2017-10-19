using System;
using System.IO;
using System.Linq;
using CommentHydra.Utilities;

namespace CommentHydra
{
    /// <summary>
    /// Adds or updates header comments for every file in a folder.
    /// </summary>
    internal class Commenter
    {
        internal string[] NonComment { get; }
        internal string Folder { get; }
        internal string[] Extensions { get; }
        internal bool Verbose { get; set; }

        /// <summary>
        /// Constructor for the commenter class, used to add/update comments to the start of every file in a given folder.
        /// </summary>
        /// <param name="folder">The folder containing the files to add/update comments to.</param>
        /// <param name="nonComment">The type of non-comments to look for at the start of a file, i.e. when to stop looking for comments.</param>
        /// <param name="extensions">Any file extensions that should have comments added to them.</param>
        /// <remarks>May need to adjust this if single instance of Commenter needs to comment across multiple file types.
        /// e.g. .cs files have different nonComment set than .cpp files, etc</remarks>
        internal Commenter(string folder, string[] nonComment, string[] extensions)
        {
            Folder = folder;
            NonComment = nonComment;
            Extensions = extensions;
            Verbose = false;
        }

        /// <summary>
        /// Constructor for the commenter class, used to add/update comments to every .cs file in a given folder.
        /// </summary>
        /// <param name="folder">The folder containing the files to add/update comments to.</param>
        internal Commenter(string folder)
        {
            Folder = folder;
            NonComment = new string[] { "using", "namespace", "class", "///" };
            Extensions = new string[] { ".cs" };
            Verbose = false;
        }

        /// <summary>
        /// Add comments to every file in a folder, and possibly all files in its subfolders.
        /// </summary>
        /// <param name="commentLines">The comments to add.</param>
        /// <param name="includeSubfolders">False = only add comments to files in the current folder.</param>
        /// <param name="replaceOldComments">False = files with existing header comments will be ignored.</param>
        internal void AddHeaderComments(string[] commentLines, bool includeSubfolders, bool replaceOldComments)
        {
            var files = FileInput.GetFiles(Folder, includeSubfolders, Extensions).ToList();
            foreach (string path in files)
            {
                if (replaceOldComments)
                {
                    ReplaceHeaderComments(path, commentLines);
                }
                else if (!HasHeaderComments(path))
                {
                    var linesToKeep = File.ReadAllLines(path).ToList();
                    linesToKeep.InsertRange(0, commentLines);
                    try
                    {
                        File.Delete(path);
                        if (Verbose)
                        {
                            Console.WriteLine("Adding comments to: " + path);
                        }
                        File.WriteAllLines(path, linesToKeep);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Program.CloseProgram();
                        throw;
                    }
                }
                else if (Verbose)
                {
                    Console.WriteLine("File already has comments, ignoring: " + path);
                }
            }
        }

        /// <summary>
        /// Remove any comments from the header (i.e. before using, namespace, etc) of a file, and replace them with a new set of comments.
        /// </summary>
        /// <param name="path">The file to remove comments from.</param>
        /// <param name="newComments">The new comments to add.</param>
        private void ReplaceHeaderComments(string path, string[] newComments)
        {
            try
            {
                var linesToKeep = File.ReadAllLines(path).ToList();
                foreach (string line in linesToKeep)
                {
                    // Trim in case of unusual whitespace style
                    // This should make it stop at any using / import / namespace / class / XML doc comment / etc.
                    if (NonComment.Any(s => line.Trim().StartsWith(s)))
                    {
                        break;
                    }
                    else
                    {
                        linesToKeep.Remove(line);
                    }
                }

                linesToKeep.InsertRange(0, newComments);

                File.Delete(path);
                if (Verbose)
                {
                    Console.WriteLine("Replacing/adding comments in: " + path);
                }
                File.WriteAllLines(path, linesToKeep);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Program.CloseProgram();
                throw;
            }
        }

        /// <summary>
        /// Checks if a given file has comments at the top of a file already.
        /// </summary>
        /// <param name="path">The file to check for header comments.</param>
        /// <returns>Returns true if the file already has comments at the top, false if not.</returns>
        private bool HasHeaderComments(string path)
        {
            try
            {
                var lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (NonComment.Any(s => line.Trim().StartsWith(s)))
                    {
                        break;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Program.CloseProgram();
                throw;
            }
            return false;
        }
    }
}
