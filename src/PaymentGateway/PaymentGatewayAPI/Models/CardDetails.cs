using System;
using System.Collections.Generic;

namespace PaymentGatewayAPI.Models
{
    public class CardDetails
    {
        public int CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; }

        public virtual ICollection<PaymentRequest> Payments { get; set; }
    }
}
