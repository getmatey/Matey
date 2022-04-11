namespace Matey.Frontend.Abstractions
{
    public interface IFrontend
    {
        string Name { get; }

        void InitializeRequestRoutes(IEnumerable<RequestRoute> routes);

        void AddRequestRoute(RequestRoute route);

        void RemoveRequestRoutes(ApplicationRequestEndpoint endpoint);
    }
}
