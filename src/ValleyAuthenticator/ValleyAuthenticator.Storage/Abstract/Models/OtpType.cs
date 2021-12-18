namespace ValleyAuthenticator.Storage.Abstract.Models
{
    public enum OtpType
    {
        /// <summary>
        /// Timed one-time password
        /// </summary>
        Totp = 1,

        /// <summary>
        /// One-time password
        /// </summary>
        Hotp = 2,
    }
}
