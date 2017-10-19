using System;

namespace CommentHydra
{
    class Program
    {
        // TODO oct 18 2017: write arg handling & hook up to actual commenter class
        // should accept at least a .txt path to use as comment source & a target folder path
        // ideally, also should accept parameters like includeSubfolders & replaceOldComments
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CloseProgram();
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
