using System;

using AwesomeAssertions;

using LighthousePlaywright.Net.Core;
using LighthousePlaywright.Net.Objects;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

namespace LighthousePlaywright.Net.Tests.Unit;

public class CategoryTests
{
    private enum NoDescription
    {
        Plain
    }

    [TestCase(Category.Accessibility, "accessibility")]
    [TestCase(Category.BestPractices, "best-practices")]
    [TestCase(Category.Performance, "performance")]
    [TestCase(Category.PWA, "pwa")]
    [TestCase(Category.SEO, "seo")]
    public void GetDescription_ReturnsLighthouseName(Category category, string expected)
    {
        category.GetDescription().Should().Be(expected);
    }

    [Test]
    public void GetDescription_WithoutAttribute_ReturnsName()
    {
        NoDescription.Plain.GetDescription().Should().Be("Plain");
    }

    [Test]
    public void GetDescription_UndefinedEnumValue_ReturnsNumericName()
    {
        ((Category)42).GetDescription().Should().Be("42");
    }

    [Test]
    public void GetDescription_NonEnum_Throws()
    {
        var act = () => 5.GetDescription();

        act.Should().Throw<ArgumentException>().WithParameterName("enumerationValue");
    }

    [Test]
    public void LighthouseJsOptions_SerializesOnlyCategoriesAsDescriptions()
    {
        var options = new LighthouseJsOptions
        {
            OnlyCategories = [Category.Performance, Category.BestPractices]
        };

        var json = JObject.Parse(JsonConvert.SerializeObject(options));

        json["onlyCategories"]!.ToObject<string[]>().Should().Equal("performance", "best-practices");
        json.ContainsKey("OnlyCategories").Should().BeFalse();
    }

    [Test]
    public void LighthouseJsOptions_NullCategories_SerializesAsNull()
    {
        new LighthouseJsOptions().onlyCategories.Should().BeNull();
    }
}
