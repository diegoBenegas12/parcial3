namespace api.primerparcial;

public class TokenSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public double ExpirationMinutes { get; set; }
}
