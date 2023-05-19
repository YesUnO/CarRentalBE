using System.ComponentModel.DataAnnotations;

namespace CarRentalAPI.Models.User;

public class DeleteUserModel
{
    [Required]
    public string Email { get; set; }
}
