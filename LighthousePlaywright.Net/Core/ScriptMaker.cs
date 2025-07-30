using LighthousePlaywright.Net.Objects;

using System.IO;

namespace LighthousePlaywright.Net.Core;

internal sealed class ScriptMaker(Options options)
{
    internal string TempFileName { get; private set; }

    internal string Produce(AuditRequest request, string npmPath)
    {
        if (request == null)
        {
            return null;
        }

        var data = GetTemplate;

        var jsOptions = new LighthouseJsOptions
        {
            ChromeFlags =
            [
                "--show-paint-rects",
                "--headless",
                "--no-sandbox"
            ],
            MaxWaitForLoad = request.MaxWaitForLoad,
            BlockedUrlPatterns = request.BlockedUrlPatterns,
            DisableStorageReset = request.DisableStorageReset,
            DisableDeviceEmulation = request.DisableDeviceEmulation,
            OnlyCategories = request.OnlyCategories,
            Preset = request.EmulatedFormFactor?.ToString().ToLower()
        };

        var optionsAsJson = JsonConvert.SerializeObject(jsOptions,
            Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        var reportsAsJson = JsonConvert.SerializeObject(options.Reports,
            Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        var thresholdsAsJson = JsonConvert.SerializeObject(options.Thresholds,
            Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        data = data.Replace("{OPTIONS}", optionsAsJson).Replace("{REPORTS}", reportsAsJson)
            .Replace("{THRESHOLDS}", thresholdsAsJson)
            .Replace("{URL}", request.Url).Replace("{PORT}", options.Port.ToString())
            .Replace("{NODE_MODULES}", $@"{npmPath.Replace("\\", @"\\")}\\node_modules");

        return data;
    }

    public bool Save(string content)
    {
        this.TempFileName = null;

        var tempPath = Path.GetTempPath();

        var fullPath = $"{tempPath}lighthouse-net-{Guid.NewGuid():N}.js";
        try
        {
            File.WriteAllText(fullPath, content);
            this.TempFileName = fullPath;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete()
    {
        if (string.IsNullOrEmpty(this.TempFileName))
        {
            return false;
        }

        try
        {
            File.Delete(this.TempFileName);
            return true;
        }
        catch (Exception)
        {
            // ignore
        }

        return false;
    }

    private const string GetTemplate =
         """

         const { playAudit } = require('{NODE_MODULES}\\playwright-lighthouse');
         const playwright = require('{NODE_MODULES}\\playwright');

         async function launchChromeAndRunLighthouse(url, opts, config = null) {
             return await playwright['chromium'].launch({
                 args: ['--remote-debugging-port={PORT}'],
             }).then(async browser => {
                 const page = await browser.newPage();
                 await page.goto(url);

                 return await playAudit({
                     page: page,
                     opts: opts,
                     thresholds: {THRESHOLDS},
                     reports: {REPORTS},
                     port: {PORT}
                 }).then(async results => {
                     await browser.close();
                     return results.lhr;
                 });
             });
         }

         const opts = {OPTIONS};

         launchChromeAndRunLighthouse('{URL}', opts).then(results => {
             console.log(JSON.stringify(results));
         });
         """;
}