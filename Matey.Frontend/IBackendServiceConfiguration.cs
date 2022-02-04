namespace Matey.Frontend
{
    public interface IBackendServiceConfiguration
    {
        string Name { get; }

        int? Port { get; }

        IFrontendServiceConfiguration Frontend { get; }
    }
}
