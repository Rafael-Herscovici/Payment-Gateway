using System;
using Common.Entities;

namespace BankEmulatorDB.Entities
{
    public class AccountEntity : BaseEntity
    {
        public int CardNumber { get; set; }
        public DateTime CardExpiryDate { get; set; }
        public int CardSecurityCode { get; set; }
        public decimal Balance { get; set; }
    }
}
