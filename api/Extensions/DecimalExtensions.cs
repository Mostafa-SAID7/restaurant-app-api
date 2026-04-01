using System.Globalization;

namespace FakeRestuarantAPI.Extensions;

public static class DecimalExtensions
{
    /// <summary>
    /// Formats decimal as currency
    /// </summary>
    public static string ToCurrency(this decimal amount, string currencyCode = "USD")
    {
        var culture = currencyCode.ToUpper() switch
        {
            "USD" => new CultureInfo("en-US"),
            "EUR" => new CultureInfo("de-DE"),
            "GBP" => new CultureInfo("en-GB"),
            _ => CultureInfo.CurrentCulture
        };

        return amount.ToString("C", culture);
    }

    /// <summary>
    /// Rounds to specified decimal places
    /// </summary>
    public static decimal RoundTo(this decimal value, int decimalPlaces = 2)
    {
        return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Calculates percentage of total
    /// </summary>
    public static decimal PercentageOf(this decimal value, decimal total)
    {
        if (total == 0)
            return 0;

        return (value / total) * 100;
    }

    /// <summary>
    /// Calculates tax amount
    /// </summary>
    public static decimal CalculateTax(this decimal amount, decimal taxRate)
    {
        return amount * (taxRate / 100);
    }

    /// <summary>
    /// Adds tax to amount
    /// </summary>
    public static decimal AddTax(this decimal amount, decimal taxRate)
    {
        return amount + amount.CalculateTax(taxRate);
    }

    /// <summary>
    /// Checks if value is between min and max (inclusive)
    /// </summary>
    public static bool IsBetween(this decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Formats as percentage
    /// </summary>
    public static string ToPercentage(this decimal value, int decimalPlaces = 1)
    {
        return $"{value.RoundTo(decimalPlaces)}%";
    }
}