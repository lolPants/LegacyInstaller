using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace LegacyInstaller.Versions
{
    internal static class VersionManager
    {
        private static List<Version> _versions = null;
        public static List<Version> Versions
        {
            get
            {
                if (_versions == null)
                {
                    _versions = GetVersions();
                }

                return _versions;
            }
        }

        private static List<Version> GetVersions()
        {
            var bsVersionsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LegacyInstaller.Resources.versions.json");
            string versionList = new StreamReader(bsVersionsStream).ReadToEnd();

            var versions = JsonConvert.DeserializeObject<List<Version>>(versionList);
            return versions;
        }
    }
}
