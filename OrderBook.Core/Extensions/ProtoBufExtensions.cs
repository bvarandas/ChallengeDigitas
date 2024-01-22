namespace OrderBook.Core.Extensions;

public static class ProtoBufExtensions
{
    public static string SerializeToStringProtobuf<T>(this T obj) where T : class
    {
        using var ms = new MemoryStream();

        ProtoBuf.Serializer.Serialize(ms, obj);

        return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
    }

    public static T DeserializeFromStringProtobuf<T>(this string txt) where T : class
    {
        var arr = Convert.FromBase64String(txt);

        using var ms = new MemoryStream();

        return ProtoBuf.Serializer.Deserialize<T>(ms);
    }

    public static byte[] SerializeToByteArrayProtobuf<T>(this T obj) where T : class
    {
        using var ms = new MemoryStream();

        ProtoBuf.Serializer.Serialize(ms, obj);
        return ms.ToArray();
    }
    public static T DeserializeFromByteArrayProtobuf<T>(this byte[] arr) where T : class
    {
        using var ms = new MemoryStream(arr);

        return ProtoBuf.Serializer.Deserialize<T>(ms);
    }
}
