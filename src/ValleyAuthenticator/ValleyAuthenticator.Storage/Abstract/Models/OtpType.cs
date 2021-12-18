namespace ValleyAuthenticator.Storage.Abstract.Models
{
    public enum OtpType
    {
        /// <summary>
        /// Timed one-time password
        /// </summary>
        Totp,

        /// <summary>
        /// One-time password
        /// </summary>
        Hotp,
    }
}
