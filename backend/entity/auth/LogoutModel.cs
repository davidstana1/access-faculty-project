using System.ComponentModel.DataAnnotations;

namespace backend.entity.auth;

public class LogoutModel
{
    [Required]
    public string RefreshToken { get; set; }
}