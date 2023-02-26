namespace Matey.Acme.Http
{
    public class AcmeHttpBinding
    {
        public string HostName { get; init; } = AcmeHttpOptionsDefaults.HostName;
        public int Port { get; init; } = AcmeHttpOptionsDefaults.Port;
    }
}
