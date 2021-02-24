namespace PaymentGateway.Services.DTOs
{
    public class PaymentDto
    {
        public string Id { get; set; }
        public string CardNumber { get; set; }
        public CardTypeDto CardType { get; set; }
        public string CardHolderName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public int Cvv { get; set; }
        public string Status { get; set; }
    }
}