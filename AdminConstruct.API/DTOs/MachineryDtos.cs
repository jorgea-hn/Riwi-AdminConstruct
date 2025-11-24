using System.ComponentModel.DataAnnotations;

namespace AdminConstruct.API.DTOs
{
    public class MachineryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMachineryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
