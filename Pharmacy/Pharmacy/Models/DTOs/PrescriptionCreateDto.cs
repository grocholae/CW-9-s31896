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

public class PrescriptionGetDto
{
    public int IdPrescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public virtual PatientDto Patient { get; set; }
    public virtual DoctorDto Doctor { get; set; }
    public ICollection<MedicamentDto> Medicaments { get; set; } = null!;
}

public class PatientDto
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
}
