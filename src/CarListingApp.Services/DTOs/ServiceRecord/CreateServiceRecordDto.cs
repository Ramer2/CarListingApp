using System.ComponentModel.DataAnnotations;

namespace CarListingApp.Services.DTOs.ServiceRecord;

public class CreateServiceRecordDto
{
    [Required]
    [Range(0, int.MaxValue)]
    public int MileageAtService { get; set; }

    [Required]
    public DateTime ServiceDate { get; set; }

    [Required]
    [Range(0.0, 5.0)]
    public decimal Grade { get; set; }
}