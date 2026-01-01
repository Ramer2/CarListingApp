namespace CarListingApp.Models.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsBlocked { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
