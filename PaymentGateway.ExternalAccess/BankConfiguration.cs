namespace PaymentGateway.ExternalAccess
{
    public class BankConfiguration : IBankConfiguration
    {
        public const string ConfigurationName = "Bank";
        public string BaseUrl { get; set; }
        public string CleintSecret { get; set; }
    }
}