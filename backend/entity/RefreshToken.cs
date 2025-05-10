namespace backend.entity;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
}