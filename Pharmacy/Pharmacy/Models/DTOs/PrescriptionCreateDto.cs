using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models.DTOs;

public class PrescriptionCreateDto
{
    public PatientGetDto Patient { get; set; }
    [Required]
    public int IdDoctor { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    public ICollection<MedicamentDto>? Medicaments { get; set; } = null!;
}
