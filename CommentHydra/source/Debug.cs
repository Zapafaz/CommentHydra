using System;

namespace CommentHydra
{
    class Debug
    {
        public static void Write(string text)
        {
#if DEBUG
            Console.Write(text);
#endif
        }

        public static void WriteLine(string text)
        {
#if DEBUG
            Console.WriteLine(text);
#endif
        }
    }
}
