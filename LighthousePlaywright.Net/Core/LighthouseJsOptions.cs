using LighthousePlaywright.Net.Objects;

using System.Collections.Generic;
using System.Linq;

namespace LighthousePlaywright.Net.Core;

internal sealed class LighthouseJsOptions
{
    [JsonProperty(PropertyName = "chromeFlags")]
    public IEnumerable<string> ChromeFlags { get; set; }

    [JsonProperty(PropertyName = "maxWaitForLoad")]
    public int? MaxWaitForLoad { get; set; }

    [JsonProperty(PropertyName = "blockedUrlPatterns")]
    public IEnumerable<string> BlockedUrlPatterns { get; set; }

    [JsonProperty(PropertyName = "disableStorageReset")]
    public bool? DisableStorageReset { get; set; }

    [JsonProperty(PropertyName = "disableDeviceEmulation")]
    public bool? DisableDeviceEmulation { get; set; }

    [JsonProperty(PropertyName = "preset")]
    public string Preset { get; set; }

    [JsonIgnore]
    public IEnumerable<Category> OnlyCategories { get; set; }

    [JsonProperty(PropertyName = "onlyCategories")]
    public IEnumerable<string> onlyCategories
    {
        get { return OnlyCategories?.Select(s => s.GetDescription()); }
    }
}