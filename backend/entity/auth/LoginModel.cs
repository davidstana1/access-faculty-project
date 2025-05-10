using System.ComponentModel.DataAnnotations;

namespace backend.entity.auth;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}