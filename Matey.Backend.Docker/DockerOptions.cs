namespace Matey.Backend.Docker
{
    public class DockerOptions
    {
        public string Endpoint { get; init; } = DockerOptionsDefaults.DockerEndpoint;

        public string LabelPrefix { get; init; } = DockerOptionsDefaults.LabelPrefix;
    }
}
