<img src="https://github.com/w8tcha/Lighthouse.Net/raw/refs/heads/master/lighthouse-logo.svg">

## Lighthouse.Net [![Nuget](https://img.shields.io/nuget/v/lighthouse.net.svg)](https://www.nuget.org/packages/lighthouse.net)
This is a .net (c#) library for [Google Lighthouse](https://github.com/GoogleChrome/lighthouse) tool.

Lighthouse.NET analyzes web apps and web pages, collecting modern performance metrics and insights on developer best practices from your code.

*Auditing, performance metrics, and best practices for Progressive Web Apps in .NET tests.*

### How to install

You need to install lighthouse as Node module on machine ([more info](https://developers.google.com/web/tools/lighthouse/)).

1. Download [Google Chrome](https://www.google.com/chrome/) for Desktop.
2. Install the current [Long-Term Support](https://github.com/nodejs/LTS) version of [Node](https://nodejs.org/).
3. Install Lighthouse. The `-g` flag installs it as a global module.
`npm install -g lighthouse`

4. Install lighthouse.net into your project via NuGet
```
PM> Install-Package lighthouse.net
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


### Known Issues
- If you installed lighthouse package with version 9.0.0 and higher it's required to use Node.js version 14 (because Optional Chaining Operator is used in lighthouse package). To install lighthouse package, that supports Node.js v12 please use `npm i lighthouse@8.6.0 -g`