using System;
using System.Collections.Generic;
using System.Text;

namespace PaysafeCheck.Skrill
{
    public class SkrillUserIdRequest
    {
        public string merchantId { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string customerId { get; set; }
    }
}
