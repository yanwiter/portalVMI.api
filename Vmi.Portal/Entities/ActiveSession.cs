namespace Vmi.Portal.Entities
{
    public class ActiveSession
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IpAddress { get; set; }
        public string DeviceInfo { get; set; }
    }
}
