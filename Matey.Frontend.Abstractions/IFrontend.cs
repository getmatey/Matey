namespace Matey.Frontend.Abstractions
{
    public interface IFrontend
    {
        string Name { get; }

        void InitializeRequestRoutes(IEnumerable<RequestRouteRule> rules);

        void AddRequestRoute(RequestRouteRule rule);

        void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint);
    }
}
