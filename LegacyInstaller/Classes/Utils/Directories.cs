using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LegacyInstaller.Utils
{
    // Code reused from ModAssistant
    // https://github.com/Assistant/ModAssistant/blob/master/LICENSE
    internal static class Directories
    {
        public static readonly string _beatSaberAppId = "620980";
        public static readonly string _beatSaberExe = "Beat Saber.exe";
        public static readonly string _steamExe = "steam.exe";

        public static string GetSteamDirectory()
        {
            string steamInstall = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)?.OpenSubKey("SOFTWARE")?.OpenSubKey("WOW6432Node")?.OpenSubKey("Valve")?.OpenSubKey("Steam")?.GetValue("InstallPath").ToString();
            if (string.IsNullOrEmpty(steamInstall))
            {
                steamInstall = Registry.LocalMachine.OpenSubKey("SOFTWARE")?.OpenSubKey("WOW6432Node")?.OpenSubKey("Valve")?.OpenSubKey("Steam")?.GetValue("InstallPath").ToString();
            }

            if (CheckSteamDirectory(steamInstall) == false)
            {
                return null;
            }

            return steamInstall;
        }

        public static bool CheckSteamDirectory(string steamInstall)
        {
            string exePath = Path.Combine(steamInstall, _steamExe);
            return File.Exists(exePath);
        }

        public static string GetBeatSaberDirectory()
        {

            var steamInstall = GetSteamDirectory();
            if (string.IsNullOrEmpty(steamInstall)) return null;

            string vdf = Path.Combine(steamInstall, @"steamapps\libraryfolders.vdf");
            if (!File.Exists(vdf)) return null;

            Regex regex = new Regex("\\s\"(?:\\d|path)\"\\s+\"(.+)\"");
            List<string> steamPaths = new List<string>
            {
                Path.Combine(steamInstall, "steamapps")
            };

            using (StreamReader reader = new StreamReader(vdf))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        steamPaths.Add(Path.Combine(match.Groups[1].Value.Replace(@"\\", @"\"), "steamapps"));
                    }
                }
            }

            regex = new Regex("\\s\"installdir\"\\s+\"(.+)\"");
            foreach (string path in steamPaths)
            {
                var acfPath = Path.Combine(@path, $"appmanifest_{_beatSaberAppId}.acf");
                if (File.Exists(acfPath))
                {
                    using (StreamReader reader = new StreamReader(acfPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Match match = regex.Match(line);
                            if (match.Success)
                            {
                                var beatSaberDir = Path.Combine(path, "common", match.Groups[1].Value);
                                if (CheckBeatSaberDirectory(beatSaberDir))
                                {
                                    return beatSaberDir;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static bool CheckBeatSaberDirectory(string beatSaberDir)
        {
            string exePath = Path.Combine(beatSaberDir, _beatSaberExe);
            return File.Exists(exePath);
        }
    }
}
