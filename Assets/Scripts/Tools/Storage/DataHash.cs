using System.Security.Cryptography;

/// <summary>
/// 数据哈希值计算类。
/// </summary>
public static class DataHash {

    public static string CreateCheck(string text) {
        return CreateSHA256(text);
    }

    private static string CreateSHA256(string text) {

        using (var sha256 = SHA256.Create()) {
            var data = System.Text.Encoding.UTF8.GetBytes(text);
            var hashData = sha256.ComputeHash(data);

            var builder = new System.Text.StringBuilder(64);
            foreach (var b in hashData) {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }

    }
}
