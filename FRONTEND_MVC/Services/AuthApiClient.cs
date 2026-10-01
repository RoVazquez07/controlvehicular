using System.Net.Http.Json;
using System.Text.Json;

namespace ControlVehicular.Web.Services;

public interface IAuthApiClient
{
    Task<AuthSession> LoginAsync(string email, string password, CancellationToken cancellationToken);

    Task LogoutAsync(string accessToken, CancellationToken cancellationToken);
}

public sealed class AuthApiException : Exception
{
    public AuthApiException(string message, int status)
        : base(message)
    {
        Status = status;
    }

    public int Status { get; }
}

public sealed record AuthSession(string AccessToken, string? RefreshToken, AuthUser User);

public sealed class AuthUser
{
    public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }
}

public sealed class AuthApiClient : IAuthApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly HttpClient _http;

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<AuthSession> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync(
                "api/auth/login",
                new { email, password },
                JsonOptions,
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new AuthApiException("No se pudo conectar con el servidor", 0);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new AuthApiException("No se pudo conectar con el servidor", 0);
        }

        var text = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new AuthApiException(ResolveMessage((int)response.StatusCode, text), (int)response.StatusCode);
        }

        LoginResponse? payload;
        try
        {
            payload = JsonSerializer.Deserialize<LoginResponse>(text, JsonOptions);
        }
        catch (JsonException)
        {
            throw new AuthApiException("No se pudo iniciar sesión", (int)response.StatusCode);
        }

        if (payload is null
            || string.IsNullOrWhiteSpace(payload.AccessToken)
            || payload.User is null
            || string.IsNullOrWhiteSpace(payload.User.Email))
        {
            throw new AuthApiException("No se pudo iniciar sesión", (int)response.StatusCode);
        }

        return new AuthSession(payload.AccessToken, payload.RefreshToken, payload.User);
    }

    public async Task LogoutAsync(string accessToken, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            await _http.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
        }
    }

    private static string ResolveMessage(int status, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            try
            {
                using var document = JsonDocument.Parse(text);
                var root = document.RootElement;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (TryReadString(root, "message", out var message))
                    {
                        return message;
                    }

                    if (TryReadString(root, "title", out var title))
                    {
                        return title;
                    }

                    if (TryReadString(root, "error", out var error))
                    {
                        return error;
                    }
                }
            }
            catch (JsonException)
            {
            }
        }

        if (status == 401)
        {
            return "Credenciales inválidas";
        }

        if (status == 403)
        {
            return "No autorizado";
        }

        if (status >= 500)
        {
            return "Error del servidor";
        }

        return $"Error de red ({status})";
    }

    private static bool TryReadString(JsonElement root, string name, out string value)
    {
        if (root.TryGetProperty(name, out var property) && property.ValueKind == JsonValueKind.String)
        {
            value = property.GetString() ?? string.Empty;
            return value.Length > 0;
        }

        value = string.Empty;
        return false;
    }

    private sealed class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public AuthUser? User { get; set; }
    }
}
