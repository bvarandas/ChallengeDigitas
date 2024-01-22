namespace OrderBook.API.Utils;

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
