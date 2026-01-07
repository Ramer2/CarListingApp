namespace CarListingApp.Models.Models;

public partial class Car
{
    public int Id { get; set; }

    public double Price { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? Color { get; set; }

    public int Year { get; set; }

    public string? Vin { get; set; }

    public double? EngineDisplacement { get; set; }

    public double? EnginePower { get; set; }

    public int? Mileage { get; set; }

    public int Seller { get; set; }

    public int Status { get; set; }

    public string? Description { get; set; }

    public virtual User SellerNavigation { get; set; } = null!;

    public virtual ICollection<ServiceRecord> ServiceRecords { get; set; } = new List<ServiceRecord>();

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}
