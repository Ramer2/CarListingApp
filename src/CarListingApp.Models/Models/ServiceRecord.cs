namespace CarListingApp.Models.Models;

public partial class ServiceRecord
{
    public int Id { get; set; }

    public int MileageAtService { get; set; }

    public DateTime ServiceDate { get; set; }

    public decimal Grade { get; set; }

    public int Car { get; set; }

    public virtual Car CarNavigation { get; set; } = null!;
}
