using System.ComponentModel.DataAnnotations;

namespace backend.entity.auth;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public int? DivisionId { get; set; }

    [Required]
    [RegularExpression("^(HR|GatePersonnel)$", ErrorMessage = "Rolul trebuie sa fie HR sau GatePersonnel")]
    public string Role { get; set; }
}