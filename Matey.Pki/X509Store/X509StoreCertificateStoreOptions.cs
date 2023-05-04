namespace Matey.Pki.X509Store
{
    public class X509StoreCertificateStoreOptions
    {
        public string StoreLocation { get; init; } = "LocalMachine";
        public string StoreName { get; init; } = "Root";
    }
}
