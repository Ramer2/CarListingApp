namespace CarListingApp.Services.DTOs.ServiceRecord;

public class ServiceRecordDto
{
    public int Id { get; set; }
    
    public int MileageAtService { get; set; }
    
    public string ServiceDate { get; set; }
    
    public decimal Grade { get; set; }
    
    public int CarId { get; set; }
}