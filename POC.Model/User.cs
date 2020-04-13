using System;

namespace POC.Model
{
    [Serializable]
    public class User
    {
        public string PaymentAccountId { get; set; }
        public string Name { get; set; }
    }
}
