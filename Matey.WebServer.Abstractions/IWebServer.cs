namespace Matey.WebServer.Abstractions
{
    public interface IWebServer
    {
        string Name { get; }

        void InitializeRequestRoutes(IEnumerable<RequestRoute> routes);

        void AddRequestRoute(RequestRoute route);

        void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint);
    }
}
