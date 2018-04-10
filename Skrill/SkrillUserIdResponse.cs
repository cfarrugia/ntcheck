using System;
using System.Collections.Generic;
using System.Text;

namespace PaysafeCheck.Skrill
{
    public class SkrillUserIdResponse
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string HouseNumber { get; set; }
        public string Country { get; set; }
        public string PostCode { get; set; }
        public string VerificationLevel { get; set; }
        public string Code { get; set; }

        public string Message { get; set; } 
        public string Parameter { get; set; }
    }
}
