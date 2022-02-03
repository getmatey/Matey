namespace Matey.Backend.Docker
{
    public class DockerOptions
    {
        public string Endpoint { get; init; } = Defaults.DOCKER_ENDPOINT;

        public string LabelPrefix { get; init; } = Defaults.LABEL_PREFIX;
    }
}
