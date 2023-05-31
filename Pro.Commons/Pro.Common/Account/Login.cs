namespace Pro.Common.Account
{
    public class UserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public long ExpiresOn { get; set; }
    }
    public class ChangePasswordRequest
    {
        public int UserID { get; set; }
        public string NewPassword { get; set; }
    }
}
