using System.ComponentModel.DataAnnotations;

namespace backend.entity.auth;

public class RefreshTokenModel
{
    [Required]
    public string RefreshToken { get; set; }
}