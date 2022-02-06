namespace Matey.Backend.Docker
{
    public class DockerOptions
    {
        public string Endpoint { get; init; } = ConfigurationDefault.DockerEndpoint;

        public string LabelPrefix { get; init; } = ConfigurationDefault.LabelPrefix;
    }
}
