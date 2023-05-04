namespace Matey.WebServer.Abstractions
{
    public record RequestRouteStickinessSettings(bool IsSticky, string CookieName);
}
