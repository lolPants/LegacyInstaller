using Newtonsoft.Json;

namespace LegacyInstaller.Versions
{
    internal class Version
    {
        [JsonProperty("BSVersion")]
        public string BeatSaberVersion { get; private set; }
        [JsonProperty("ManifestId")]
        public string ManifestId { get; private set; }
        [JsonProperty("ReleaseURL")]
        public string ReleaseURL { get; private set; }

        public override string ToString()
        {
            return BeatSaberVersion;
        }
    }
}
