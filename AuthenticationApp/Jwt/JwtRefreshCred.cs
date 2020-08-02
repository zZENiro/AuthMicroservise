namespace AuthenticationApp.Jwt
{
    public class JwtRefreshCred : IRefreshCred
    {
        public string JwtToken { get; set; }
        public string JwtRefreshToken { get; set; }
    }
}
