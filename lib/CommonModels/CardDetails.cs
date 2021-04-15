using System;

namespace CommonModels
{
    public class CardDetails
    {
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CVV { get; set; }
    }
}
