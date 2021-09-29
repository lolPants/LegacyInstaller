using System;
using System.Collections.Generic;
using System.IO;

namespace LegacyInstaller.Utils
{
    internal static class IO
    {
        public static string[] EnumerateDirectory(string directoryName, bool relative)
        {
            var files = Directory.GetFiles(directoryName, "*", SearchOption.AllDirectories);
            var output = new List<string>();

            foreach (var file in files)
            {
                if (!File.Exists(file)) continue;

                if (relative)
                {
                    var rel = GetRelativePath(directoryName, file);
                    output.Add(rel);
                }
                else
                {
                    output.Add(file);
                }
            }

            return output.ToArray();
        }

        private static string GetRelativePath(string relativeTo, string path)
        {
            var uri = new Uri(relativeTo);
            var rel = Uri.UnescapeDataString(uri.MakeRelativeUri(new Uri(path)).ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (rel.Contains(Path.DirectorySeparatorChar.ToString()) == false)
            {
                rel = $".{Path.DirectorySeparatorChar}{rel}";
            }

            return rel;
        }
    }
}
