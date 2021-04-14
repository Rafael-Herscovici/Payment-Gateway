using System;

namespace CommonModels
{
    public class CardPayment
    {
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ushort CVV { get; set; }
    }
}
