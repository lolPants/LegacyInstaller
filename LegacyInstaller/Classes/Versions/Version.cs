using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LegacyInstaller.Versions
{
    internal class Version
    {
        [JsonProperty("beatSaberVersion")]
        public string BeatSaberVersion { get; private set; }

        [JsonProperty("manifestId")]
        public string ManifestId { get; private set; }

        [JsonProperty("releaseTime")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime ReleaseTime { get; private set; }

        [JsonProperty("releaseId")]
        public string ReleaseId { get; private set; }

        [JsonIgnore]
        public string ReleaseUrl => $"https://steamcommunity.com/games/620980/announcements/detail/{ReleaseId}";

        public override string ToString()
        {
            return BeatSaberVersion;
        }
    }
}
