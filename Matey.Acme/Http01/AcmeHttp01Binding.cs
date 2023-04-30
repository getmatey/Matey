namespace Matey.Acme.Http01
{
    public class AcmeHttp01Binding
    {
        public string HostName { get; init; } = AcmeHttp01OptionsDefaults.HostName;
        public int Port { get; init; } = AcmeHttp01OptionsDefaults.Port;
    }
}
