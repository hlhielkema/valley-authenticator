namespace ValleyAuthenticator.Storage.Models
{
    public sealed class OtpData
    {
        public string Label { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }
    }
}
