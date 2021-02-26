using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.API.ViewModels
{
    /// <summary>
    /// Payment Request View Model
    /// </summary>
    public class PaymentRequestVm
    {
        /// <summary>
        /// Tyep of the Card. Ex: Master, Visa, Amex.
        /// </summary>
        [Required]
        [EnumDataType(typeof(CardTypeVm))]
        public CardTypeVm CardType { get; set; }

        /// <summary>
        /// 16 digit card number. Expected Format : XXXX-XXXX-XXXX-XXXX
        /// </summary>
        [Required]
        [StringLength(19)]
        public string CardNumber { get; set; }

        /// <summary>
        /// Name of the card holder.
        /// </summary>
        [Required]
        public string CardHolderName { get; set; }

        /// <summary>
        /// 1st Line of the Address.
        /// </summary>
        [Required]
        public string Address1 { get; set; }

        /// <summary>
        /// 2st Line of the Address.
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City of the Address.
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Post Code of the address.
        /// </summary>
        [Required]
        public string PostCode { get; set; }

        /// <summary>
        /// Country of the card address.
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Month of Card Expiry.
        /// </summary>
        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// Year of Card Expiry.
        /// </summary>
        [Required]
        [Range(2020, 2025)]
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Payment Amount.
        /// </summary>
        [Required]
        [Range(1, 10000)]
        public int Amount { get; set; }

        /// <summary>
        /// Payment Currency.
        /// </summary>
        [Required]
        public string Currency { get; set; }

        /// <summary>
        /// Card Verification Value (CVV)
        /// </summary>
        [Required]
        [Range(001, 999)]
        public int Cvv { get; set; }

        /// <summary>
        /// Payment status.
        /// </summary>
        public string Status { get; set; }
    }
}