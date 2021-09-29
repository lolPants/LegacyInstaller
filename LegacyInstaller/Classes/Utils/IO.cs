using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public static async Task CopyDirectoryAsync(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var file in Directory.GetFiles(source))
            {
                var destinationFile = Path.Combine(destination, Path.GetFileName(file));

                using (var fileSource = File.OpenRead(file))
                using (var fileTarget = File.Create(destinationFile))
                {
                    await fileSource.CopyToAsync(fileTarget);
                }
            }

            foreach (var directory in Directory.GetDirectories(source))
            {
                var target = Path.Combine(destination, Path.GetFileName(directory));
                await CopyDirectoryAsync(directory, target);
            }
        }
    }
}
