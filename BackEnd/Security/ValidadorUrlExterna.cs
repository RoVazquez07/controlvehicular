using System.Net;
using System.Net.Sockets;

namespace ControlVehicular.Security;

public static class ValidadorUrlExterna
{
    public static bool EsUrlRemotaPermitida(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || !Uri.TryCreate(valor, UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (uri.IsLoopback || string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!IPAddress.TryParse(uri.Host, out var ip))
        {
            return true;
        }

        return !EsIpPrivada(ip);
    }

    private static bool EsIpPrivada(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        if (ip.IsIPv4MappedToIPv6)
        {
            return EsIpPrivada(ip.MapToIPv4());
        }

        var bytes = ip.GetAddressBytes();
        if (ip.AddressFamily == AddressFamily.InterNetwork && bytes.Length == 4)
        {
            return bytes[0] == 10
                   || (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                   || (bytes[0] == 192 && bytes[1] == 168)
                   || bytes[0] == 127
                   || (bytes[0] == 169 && bytes[1] == 254);
        }

        if (ip.AddressFamily == AddressFamily.InterNetworkV6 && bytes.Length == 16)
        {
            if ((bytes[0] & 0xFE) == 0xFC)
            {
                return true;
            }

            if (bytes[0] == 0xFE && (bytes[1] & 0xC0) == 0x80)
            {
                return true;
            }
        }

        return false;
    }
}
