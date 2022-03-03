using System.Security.Cryptography;
using System.Text;

namespace DatabaseExtension;

public static class GuidCreator
{
    public static Guid CreateFrom(string value)
    {
        using HMAC md5 = CreateHMAC();

        byte[] hash = md5
            .ComputeHash(Encoding.UTF8.GetBytes(value ?? Guid.NewGuid().ToString()))
            .Take(16)
            .ToArray();

        return new Guid(hash);
    }

    public static Guid CreateFrom(double value)
    {
        using HMAC md5 = CreateHMAC();

        byte[] vs = BitConverter.GetBytes(value);

        byte[] hash = md5.ComputeHash(vs);

        return new Guid(hash);
    }

    public static Guid CreateFrom(int value)
    {
        using HMAC md5 = CreateHMAC();

        byte[] vs = BitConverter.GetBytes(value);

        byte[] hash = md5.ComputeHash(vs);

        return new Guid(hash);
    }

    private static HMAC CreateHMAC()
    {
        return new HMACSHA256(Encoding.UTF8.GetBytes(nameof(GuidCreator)));
    }
}

