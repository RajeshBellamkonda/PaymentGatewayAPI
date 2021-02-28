namespace PaymentGateway.ExternalAccess
{
    public interface IBankConfiguration
    {
        public string BaseUrl { get; set; }
        public string CleintSecret { get; set; }
        
    }
}