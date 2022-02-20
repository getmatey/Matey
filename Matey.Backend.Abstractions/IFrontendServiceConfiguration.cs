using Matey.Backend.Abstractions.Rules;

namespace Matey.Backend.Abstractions
{
    public interface IFrontendServiceConfiguration
    {
        IFrontendRule Rule { get; }
    }
}
