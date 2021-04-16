using System;

namespace PaymentGatewayDB.Migrations
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; }
        public DateTime UpdatedDate { get; }
    }
}
