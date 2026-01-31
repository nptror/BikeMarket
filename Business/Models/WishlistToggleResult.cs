namespace Business.Models;

public class WishlistToggleResult
{
    public bool Success { get; init; }
    public bool IsWishlisted { get; init; }
    public string? ErrorMessage { get; init; }
}
