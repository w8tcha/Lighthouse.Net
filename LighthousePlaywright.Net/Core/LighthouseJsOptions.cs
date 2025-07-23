using System.Collections.Generic;
using System.Linq;

using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json;

namespace LighthousePlaywright.Net.Core;

internal sealed class LighthouseJsOptions
{
    public IEnumerable<string> chromeFlags { get; set; }
    public int? maxWaitForLoad { get; set; }
    public IEnumerable<string> blockedUrlPatterns { get; set; }
    public bool? disableStorageReset { get; set; }
    public bool? disableDeviceEmulation { get; set; }

    /// <summary>
    /// For LH  version 7.0
    /// </summary>
    public string emulatedFormFactor { get; set; }

    /// <summary>
    /// For LH >= version 7.0
    /// </summary>
    public string preset { get; set; }

    [JsonIgnore] public IEnumerable<Category> OnlyCategories { get; set; }

    public IEnumerable<string> onlyCategories
    {
        get { return OnlyCategories?.Select(s => s.GetDescription()); }
    }
}