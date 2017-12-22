using System;
using System.Collections.Generic;
using System.Text;

namespace CommentHydra
{
    static class Data
    {
        private static Dictionary<string, string[]> nonComment = 
            new Dictionary<string, string[]>()
            {
                { ".cs", new string[]{"///", "namespace", "class", "using", "public", "internal", "static" } }
            };

        public static string[] GetNonComments(string extension)
        {
            if (nonComment.ContainsKey(extension))
            {
                return nonComment[extension];
            }
            throw new NotImplementedException("Cannot handle files with extension " + extension);
        }
    }
}
