namespace ValleyAuthenticator.Storage.Models
{
    public enum EntryType
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
