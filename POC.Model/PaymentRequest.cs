using System;

namespace POC.Model
{
    [Serializable]
    public class PaymentRequest
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public User User { get; set; }
    }
}
