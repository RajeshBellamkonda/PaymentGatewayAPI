namespace PaymentGateway.API.ViewModels
{
    /// <summary>
    /// Payment details View Model
    /// </summary>
    public class PaymentSummaryVm
    {
        /// <summary>
        /// Unique Identifier for the payment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Card number masked for security.
        /// </summary>
        public string MaskedCardNumber { get; set; }

        /// <summary>
        /// Tyep of the Card. Ex: Master, Visa, Amex.
        /// </summary>
        public CardTypeVm CardType { get; set; }

        /// <summary>
        /// Name of the card holder.
        /// </summary>
        public string CardHolderName { get; set; }

        /// <summary>
        /// 1st Line of the Address.
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// 2st Line of the Address.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City of the Address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Post Code of the address.
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Country of the card address.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Month of Card Expiry.
        /// </summary>
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// Year of Card Expiry.
        /// </summary>
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Payment Amount.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Payment Currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Card Verification Value (CVV)
        /// </summary>
        public int Cvv { get; set; }

        /// <summary>
        /// Payment status.
        /// </summary>
        public string Status { get; set; }
    }
}