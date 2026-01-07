namespace CarListingApp.Models.Models;

public partial class ServiceRecord
{
    public int Id { get; set; }

    public int MileageAtService { get; set; }

    public DateOnly ServiceDate { get; set; }

    public double Grade { get; set; }

    public int Car { get; set; }

    public virtual Car CarNavigation { get; set; } = null!;
}
