using System;
using System.Collections.Generic;
using System.Text;

namespace PaysafeCheck
{
    public enum VerificationLevel
    {
        NotProcessedYet = -2,
        UnknownError = -1, 
        EmailInvalid = 0,
        EmailVerificationServiceFailed = 1,
        EmailNotDeliverable = 2,
        EmailVerified = 3,
        UserNotFound = 4,
        AccountVerified = 5, 
        AccountIsActive = 6

    }

    public class UserVerificationResponse
    {
        public string Error { get; set; }

        public string Payload { get; set; }

        public VerificationLevel VerificationLevel { get; set; } = VerificationLevel.NotProcessedYet;

        public string Email { get; set; }

        public string AccountId { get; set; }

    }
}
