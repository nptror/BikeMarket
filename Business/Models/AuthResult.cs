using DataAccess.Models;

namespace Business.Models;

public class AuthResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User { get; init; }
}
