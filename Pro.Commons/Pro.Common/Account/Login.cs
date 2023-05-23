namespace Pro.Common.Account
{
    public class UserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public long ExpiresOn { get; set; }
    }
}
