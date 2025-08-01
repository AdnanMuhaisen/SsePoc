using System.ComponentModel.DataAnnotations;

namespace SsePoc.Api.Application.Commands;

public class CreateProductCommand
{
    [Required]
    [MinLength(5), MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal UnitPrice { get; set; }
}