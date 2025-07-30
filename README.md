# LighthousePlaywright.Net
<br>
  <p align="center">
      <img src="https://github.com/w8tcha/Lighthouse.Net/raw/refs/heads/master/lighthouse-logo.svg" alt="logo">
  <p align="center">
    Auditing, performance metrics, and best practices for Progressive Web Apps in .NET tests.
  </p>
<br>

[![Nuget](https://img.shields.io/nuget/v/LighthousePlaywright.Net.svg)](https://www.nuget.org/packages/LighthousePlaywright.Net)
This is a .NET (c#) library for [Google Lighthouse](https://github.com/GoogleChrome/lighthouse) tool.

[Lighthouse](https://developers.google.com/web/tools/lighthouse) is a tool developed by Google that analyzes web apps and web pages, collecting modern performance metrics and insights on developer best practices.

[Playwright](https://www.npmjs.com/package/playwright) is a Node library to automate Chromium, Firefox and WebKit with a single API. Playwright is built to enable cross-browser web automation that is ever-green, capable, reliable and fast.

> [!NOTE]  
> Project based on https://github.com/dima-horror/lighthouse.net but instead of the need for an installed version of chrome it uses playwright.

### How to install

You need to install lighthouse and playwright-lighthouse as Node module on machine ([more info](https://developers.google.com/web/tools/lighthouse/)).

2. Install the current version of [Node](https://nodejs.org/).
3. Install Lighthouse and playwright-lighthouse. The `-g` flag installs it as a global modules.

``` cmd
> npm install -g lighthouse playwright-lighthouse
```

4. Install lighthouse.net into your project via NuGet
``` cmd
PM> Install-Package LighthousePlaywright.Net
```

### Basic example

```csharp
public class LighthouseTest
{
    [Fact]
    public async Task ExampleComAudit()
    {
        var lh = new Lighthouse();
        var res = await lh.Run("http://example.com");
		
        res.Should().NotBeNull();
        res.Performance.Should().NotBeNull();
        (res.Performance > 0.5m).Should().BeTrue();

        res.Accessibility.Should().NotBeNull();
        (res.Accessibility > 0.5m).Should().BeTrue();

        res.BestPractices.Should().NotBeNull();
        (res.BestPractices > 0.5m).Should().BeTrue();

        res.Seo.Should().NotBeNull();
        (res.Seo > 0.2m).Should().BeTrue();
    }
}
```

## Options
You can parse options to the lighthouse constructor..

```csharp
 var options = new Options
 {
     Port = 9222
 };

 var lh = new Lighthouse(options);
```

### Port
The Chrome Port - **Default:** `9222`

### Thresholds
```csharp
 var options = new Options
{
    Thresholds = new Thresholds
    {
        Performance = 50, 
        Accessibility = 50, 
        BestPractices = 50, 
        Seo = 50, 
        Pwa = 50
    }
};

 var lh = new Lighthouse(options);
```

If you don't provide any threshold argument to the playAudit command, the test will fail if at least one of your metrics is under 50.

### Reports
This library can produce Lighthouse CSV, HTML and JSON audit reports. These reports can be useful for ongoing audits and monitoring from build to build.

```csharp
 var options = new Options
{
    Reports = new Reports
    {
        Formats = new Formats
        {
            Json = true, //defaults to false
            Html = true, //defaults to false
            Csv = true //defaults to false
        },
        Name = "name-of-the-report", // defaults to $"lighthouse-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}"
        Directory = "path/to/directory" // defaults to '../lighthouse'
    }
};

 var lh = new Lighthouse(options);
```