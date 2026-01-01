namespace CarListingApp.Models.Models;

public partial class UserFavorite
{
    public int UserId { get; set; }

    public int CarId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool PriceChangeNotify { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
