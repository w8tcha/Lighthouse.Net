using System;
using System.ComponentModel;

namespace Lighthouse.Net.Objects;

/// <summary>
/// Enum Category
/// </summary>
public enum Category : byte
{
    /// <summary>
    /// The accessibility
    /// </summary>
    [Description("accessibility")] Accessibility,

    /// <summary>
    /// The best practices
    /// </summary>
    [Description("best-practices")] BestPractices,

    /// <summary>
    /// The performance
    /// </summary>
    [Description("performance")] Performance,

    /// <summary>
    /// The pwa
    /// </summary>
    [Description("pwa")] PWA,

    /// <summary>
    /// The seo
    /// </summary>
    [Description("seo")] SEO
}

/// <summary>
/// Class CategoryExt.
/// </summary>
internal static class CategoryExt
{
    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerationValue">The enumeration value.</param>
    /// <returns>System.String.</returns>
    /// <exception cref="ArgumentException">EnumerationValue must be of Enum type - enumerationValue</exception>
    internal static string GetDescription<T>(this T enumerationValue) where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
        }

        var memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo.Length <= 0)
        {
            return enumerationValue.ToString();
        }

        var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumerationValue.ToString();
    }
}