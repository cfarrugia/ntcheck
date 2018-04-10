using System;
using System.Collections.Generic;
using System.Text;

namespace PaysafeCheck
{
    public enum VerificationLevel
    {
        NotProcessedYet = -2,
        UnknownError = -1, 
        UserNotFound = 1,
        AccountVerified = 2, 
        AccountIsActive = 3

    }

    public class UserVerificationResponse
    {
        public string Error { get; set; }

        public string Payload { get; set; }

        public VerificationLevel VerificationLevel { get; set; }

        public string Email { get; set; }

        public string AccountId { get; set; }

    }
}
