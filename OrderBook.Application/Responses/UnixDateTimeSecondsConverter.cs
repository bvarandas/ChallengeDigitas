using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace OrderBook.Application.Responses;

internal class UnixDateTimeSecondsConverter : DateTimeConverterBase
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var subtracted = ((DateTime)value).Subtract(UnixTime.UnixBase);
        writer.WriteRawValue(subtracted.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;

        if (reader.Value is string valueS && long.TryParse(valueS, out var valueSl))
            return UnixTime.ConvertToTimeFromSeconds(valueSl);

        if (reader.Value is long valueL)
            return UnixTime.ConvertToTimeFromSeconds(valueL);

        return null;
    }
}

internal class UnixDateTimeMillisecondsConverter : DateTimeConverterBase
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var subtracted = ((DateTime)value).Subtract(UnixTime.UnixBase);
        writer.WriteRawValue(subtracted.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;

        if (reader.Value is string valueS && long.TryParse(valueS, out var valueSl))
            return UnixTime.ConvertToTimeFromMilliseconds(valueSl / 1000);

        if (reader.Value is long valueL)
            return UnixTime.ConvertToTimeFromMilliseconds(valueL / 1000);

        return null;
    }
}

internal static class UnixTime
{
    public static readonly DateTime UnixBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long NowMs()
    {
        var subtracted = DateTime.UtcNow.Subtract(UnixBase);
        return (long)subtracted.TotalMilliseconds;
    }

    public static long NowTicks()
    {
        return DateTime.UtcNow.Ticks - UnixBase.Ticks;
    }

    public static DateTime ConvertToTimeFromMilliseconds(long timeInMs)
    {
        return UnixBase.AddMilliseconds(timeInMs);
    }

    public static DateTime? ConvertToTimeFromMilliseconds(long? timeInMs)
    {
        if (!timeInMs.HasValue)
            return null;
        return UnixBase.AddMilliseconds(timeInMs.Value);
    }

    public static DateTime ConvertToTimeFromSeconds(long timeInSec)
    {
        return UnixBase.AddSeconds(timeInSec);
    }

    public static DateTime? ConvertToTimeFromSeconds(long? timeInMs)
    {
        if (!timeInMs.HasValue)
            return null;
        return UnixBase.AddSeconds(timeInMs.Value);
    }
}
internal static class CryptoPairsHelper
{
    public static string Clean(string pair)
    {
        return (pair ?? string.Empty)
            .Trim()
                .ToLower()
                .Replace("/", "")
                .Replace("-", "")
                .Replace("\\", "");

    }
}