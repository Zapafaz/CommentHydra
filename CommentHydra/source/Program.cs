using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace CommentHydra
{
    class Program
    {
        // TODO: dec 22 2017: rewrite this with some async for practice
        // maybe rewrite Commenter so it works on a per-file basis?

        static void Main(string[] args)
        {
            try
            {
                ProcessArgs(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            CloseProgram();
        }

        private static void ProcessArgs(string[] args)
        {
            string errorHelp = "For path arguments with spaces, use \"quotation marks\" e.g. \"C:\\My Dir\\";
            string sourcePath="";
            string targetFolder ="";
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            bool replaceOldComments = false;
            for (int i = 0; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "-r":
                    case "-replace":
                        replaceOldComments = true;
                        break;
                    case "-a":
                    case "-all":
                        searchOption = SearchOption.AllDirectories;
                        break;
                    case "-s":
                    case "-source":
                        if (i + 1 < args.Length)
                        {
                            sourcePath = args[i+1];
                            if (File.Exists(sourcePath))
                            {
                                break;
                            }
                        }
                        throw new FormatException("Argument after -s(ource) should be a file path\n"+errorHelp+"file.txt\"");
                    case "-t":
                    case "-target":
                        if (i + 1 < args.Length)
                        {
                            targetFolder = args[i + 1];
                            if (Directory.Exists(targetFolder))
                            {
                                break;
                            }
                        }
                        throw new FormatException("Argument after -t(arget) should be a directory (folder) path.\n"+errorHelp+"\"");
                    default:
                        Debug.WriteLine(args[i]);
                        break;
                }
            }

            if (targetFolder == "")
            {
                targetFolder = Environment.CurrentDirectory;
            }

            string extension = "";
            string[] nonComments = null;
            string[] newComments = null;
            if (File.Exists(sourcePath) && Path.HasExtension(sourcePath))
            {
                extension = Path.GetExtension(sourcePath);
                nonComments = Data.GetNonComments(extension);
                newComments = GetAllLines(sourcePath);
            }
            else
            {
                throw new FileNotFoundException("No source file found for comments\n"+errorHelp+"comments.cs\"");
            }

            InsertComments(new Commenter(targetFolder, nonComments, extension, newComments), searchOption, replaceOldComments);
        }

        internal static void InsertComments(Commenter commenter, SearchOption searchOption, bool replaceOldComments)
        {
            commenter.AddCommentsToAllFiles(searchOption, replaceOldComments);
        }

        private static string[] GetAllLines(string path)
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(path))
            {
                while (reader.Peek() > -1)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines.ToArray();
        }

        /// <summary>
        /// A method to prompt the user to close the program.
        /// </summary>
        internal static void CloseProgram()
        {
            Console.WriteLine("Press Enter to exit the program...");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
