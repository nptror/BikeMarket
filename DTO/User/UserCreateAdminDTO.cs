namespace DTO.User;

public class UserCreateAdminDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Phone { get; set; }
    public string Role { get; set; } = "buyer"; // buyer, seller, admin
}
