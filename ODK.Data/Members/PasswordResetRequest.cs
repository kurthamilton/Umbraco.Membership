using System;

namespace ODK.Data.Members
{
    public class PasswordResetRequest
    {
        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public int MemberId { get; set; }

        public int PasswordResetRequestId { get; set; }

        public string Token { get; set; }
    }
}
