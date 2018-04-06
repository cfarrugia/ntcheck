using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PaysafeCheck.Skrill
{
    public class SkrillImpl : IUserVerifier
    {
        public async Task<UserVerificationResponse> VerifyUser(string email, string accountId)
        {
            throw new NotImplementedException();
        }
    }
}
