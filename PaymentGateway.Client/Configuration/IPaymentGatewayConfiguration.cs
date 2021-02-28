namespace PaymentGateway.Client.Configuration
{
    public interface IPaymentGatewayConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string AuthServerUrl { get; set; }
        

    }
}