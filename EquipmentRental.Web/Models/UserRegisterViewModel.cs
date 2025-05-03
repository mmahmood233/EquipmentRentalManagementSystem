using System.ComponentModel.DataAnnotations;

public class UserRegisterViewModel
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public int RoleId { get; set; } // Use dropdown: Admin=1, Manager=2, Customer=3
}
