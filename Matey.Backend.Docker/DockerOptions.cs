namespace Matey.Backend.Docker
{
    public class DockerOptions
    {
        public string Endpoint { get; init; } = DockerConfigurationDefault.DockerEndpoint;

        public string LabelPrefix { get; init; } = DockerConfigurationDefault.LabelPrefix;
    }
}
