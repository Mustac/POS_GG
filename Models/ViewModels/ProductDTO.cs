using POS_OS_GG.Helpers;
using System.ComponentModel.DataAnnotations;

namespace POS_OS_GG.Models.ViewModels;

public class ProductInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public int Measurement { get; set; } = (int)POS_OS_GG.Helpers.Measurement.Pcs;
    public float Quantity { get; set; }


}

public class ProductRegistration
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be from 3 to 50 characters")]
    public string Name { get; set; } = string.Empty;
}