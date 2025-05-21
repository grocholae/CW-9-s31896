using Microsoft.EntityFrameworkCore;
using Pharmacy.Models;

namespace Pharmacy.Data;

public class MedicalDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Patients
        var patients = new List<Patient>
        {
            new() { IdPatient = 1, FirstName = "Anna", LastName = "Kowalska", Birthdate = new DateTime(1990, 1, 1) },
            new() { IdPatient = 2, FirstName = "Jan", LastName = "Nowak", Birthdate = new DateTime(1985, 5, 20) }
        };

        // Doctors
        var doctors = new List<Doctor>
        {
            new() { IdDoctor = 1, FirstName = "Marek", LastName = "Zielinski", Email = "marek.zielinski@hospital.com" },
            new() { IdDoctor = 2, FirstName = "Ewa", LastName = "Lewandowska", Email = "ewa.lewandowska@hospital.com" }
        };

        // Medicaments
        var medicaments = new List<Medicament>
        {
            new() { IdMedicament = 1, Name = "Paracetamol", Description = "Pain reliever", Type = "Tablet" },
            new() { IdMedicament = 2, Name = "Amoxicillin", Description = "Antibiotic", Type = "Capsule" }
        };

        // Prescriptions
        var prescriptions = new List<Prescription>
        {
            new()
            {
                IdPrescription = 1,
                Date = new DateTime(2023, 1, 10),
                DueDate = new DateTime(2023, 2, 10),
                IdPatient = 1,
                IdDoctor = 1
            },
            new()
            {
                IdPrescription = 2,
                Date = new DateTime(2023, 3, 15),
                DueDate = new DateTime(2023, 4, 15),
                IdPatient = 2,
                IdDoctor = 2
            }
        };

        // PrescriptionMedicaments
        var prescriptionMedicaments = new List<PrescriptionMedicament>
        {
            new()
            {
                IdPrescription = 1,
                IdMedicament = 1,
                Dose = 500,
                Details = "Take twice daily"
            },
            new()
            {
                IdPrescription = 1,
                IdMedicament = 2,
                Dose = 250,
                Details = "Take once daily"
            },
            new()
            {
                IdPrescription = 2,
                IdMedicament = 1,
                Dose = 500,
                Details = "Before sleep"
            }
        };

        modelBuilder.Entity<Patient>().HasData(patients);
        modelBuilder.Entity<Doctor>().HasData(doctors);
        modelBuilder.Entity<Medicament>().HasData(medicaments);
        modelBuilder.Entity<Prescription>().HasData(prescriptions);
        
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdMedicament, pm.IdPrescription });

        modelBuilder.Entity<PrescriptionMedicament>().HasData(prescriptionMedicaments);
    }
}