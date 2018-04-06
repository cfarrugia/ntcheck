using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaysafeCheck
{
    public interface IUserVerifier
    {
        Task<UserVerificationResponse> VerifyUser(string email, string accountId);

    }
}
