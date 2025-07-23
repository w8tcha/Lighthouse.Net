using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json;

using System;
using System.IO;

namespace LighthousePlaywright.Net.Core;

internal sealed class ScriptMaker
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
            chromeFlags =
            [
                "--show-paint-rects",
                "--headless",
                "--no-sandbox"
            ],
            maxWaitForLoad = request.MaxWaitForLoad,
            blockedUrlPatterns = request.BlockedUrlPatterns,
            disableStorageReset = request.DisableStorageReset,
            disableDeviceEmulation = request.DisableDeviceEmulation,
            OnlyCategories = request.OnlyCategories,
            preset = request.EmulatedFormFactor?.ToString().ToLower()
        };

        var optionsAsJson = JsonConvert.SerializeObject(jsOptions,
            Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        data = data.Replace("{OPTIONS}", optionsAsJson)
            .Replace("{URL}", request.Url)
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
                 args: ['--remote-debugging-port=9222'],
             }).then(async browser => {
                 const page = await browser.newPage();
                 await page.goto(url);

                 return await playAudit({
                     page: page,
                     opts: opts,
                     thresholds: {
                         performance: 20,
                         accessibility: 20,
                         'best-practices': 20,
                         seo: 20,
                         pwa: 20
                     },
                     reports: {
                         formats: {
                             html: true,
                             json: true,
                             csv: true
                         }
                     },
                     port: 9222
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