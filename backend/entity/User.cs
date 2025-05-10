using Microsoft.AspNetCore.Identity;

namespace backend.entity;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? DivisionId { get; set; }
    public Division Division { get; set; }
}