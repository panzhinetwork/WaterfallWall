
/// <summary>
/// 数据混淆类。
/// </summary>
public static class Obfuscator {

    public static string Encode(string data) {
        return Encode(data, System.Text.Encoding.UTF8);
    }

    public static string Decode(string data) {
        return Decode(data, System.Text.Encoding.UTF8);
    }

    public static string Encode(string data, System.Text.Encoding fromEncoding) {
        var dataBytes = fromEncoding.GetBytes(data);
        return System.Convert.ToBase64String(dataBytes);
    }

    public static string Decode(string data, System.Text.Encoding toEncoding) {
        var dataBytes = System.Convert.FromBase64String(data);
        return toEncoding.GetString(dataBytes);
    }
}
