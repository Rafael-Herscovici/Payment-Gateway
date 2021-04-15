using System;

namespace CommonModels
{
    public class CardDetails
    {
        public string CardNumber { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; } = null!;
        public string CVV { get; set; } = null!;
    }
}
