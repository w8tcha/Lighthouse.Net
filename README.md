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