using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Exceptions;
using Pharmacy.Models;
using Pharmacy.Models.DTOs;

namespace Pharmacy.Services;

public interface IDbService
{
    public Task<PatientGetDto> GetPatientInfoAsync(int patientId);
    public Task<PrescriptionDto> GetPrescriptionInfoAsync(int Id);
    public Task <PrescriptionDto> CreatePrescriptionAsync(PrescriptionCreateDto prescription);
}

public class DbService(MedicalDbContext data) : IDbService
{
    public async Task<PatientGetDto> GetPatientInfoAsync(int patientId)
    {
        var result = await data.Patients
            .Select(s => new PatientGetDto
            {
                IdPatient = s.IdPatient,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Birthdate = s.Birthdate,
                Prescriptions = s.Prescriptions
                    .OrderBy(p => p.DueDate)
                    .Select(p => new PrescriptionDto
                    {
                        IdPrescription = p.IdPrescription,
                        Date = p.Date,
                        DueDate = p.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = p.Doctor.IdDoctor,
                            FirstName = p.Doctor.FirstName,
                            LastName = p.Doctor.LastName,
                            Email = p.Doctor.Email
                        },
                        Medicaments = p.PrescriptionMedicaments
                            .Select(pm => new MedicamentDto
                            {
                                IdMedicament = pm.Medicament.IdMedicament,
                                Name = pm.Medicament.Name,
                                Description = pm.Medicament.Description,
                                Type = pm.Medicament.Type,
                                Dose = pm.Dose,
                                Details = pm.Details
                            })
                            .ToList()
                    })
                    .ToList()
            }).FirstOrDefaultAsync(e => e.IdPatient == patientId);
        
        return result ?? throw new NotFoundException($"Patient with id: {patientId} not found");
    }

    public async Task<PrescriptionDto> GetPrescriptionInfoAsync(int Id)
    {
       var result = await data.Prescriptions
           .Select(s => new PrescriptionDto
           {
               IdPrescription = s.IdPrescription,
               Date = s.Date,
               DueDate = s.DueDate,
               Doctor = new DoctorDto
               {
                   IdDoctor = s.Doctor.IdDoctor,
                   FirstName = s.Doctor.FirstName,
                   LastName = s.Doctor.LastName,
                   Email = s.Doctor.Email
               },
               Medicaments = s.PrescriptionMedicaments.Select( pm => new MedicamentDto
               {
                   IdMedicament = pm.Medicament.IdMedicament,
                   Name = pm.Medicament.Name,
                   Description = pm.Medicament.Description,
                   Type = pm.Medicament.Type,
                   Dose = pm.Dose,
                   Details = pm.Details
               }).ToList()
           }).FirstOrDefaultAsync(p => p.IdPrescription == Id);
        
       return result ?? throw new NotFoundException($"Prescription with id: {Id} not found");
    }

    public async Task<PrescriptionDto> CreatePrescriptionAsync(PrescriptionCreateDto prescriptionData)
    {
        await using var transaction = await data.Database.BeginTransactionAsync();
        try
        {
            // check na pacjenta czy istnieje
            var patientId = prescriptionData.Patient.IdPatient;
            var patient = await data.Patients.FirstOrDefaultAsync(p => p.IdPatient == patientId);
            if (patient == null)
            {
                //Logika dodania pacjenta
                patient = new Patient
                {
                    FirstName = prescriptionData.Patient.FirstName,
                    LastName = prescriptionData.Patient.LastName,
                    Birthdate = prescriptionData.Patient.Birthdate,
                    Prescriptions = []
                };
                data.Patients.Add(patient);
                await data.SaveChangesAsync();
            }

            // check czy istnieja podane leki
            if (prescriptionData.Medicaments is not null && prescriptionData.Medicaments.Count != 0)
            {
                int count = 0;
                foreach (var med in prescriptionData.Medicaments)
                {
                    count++;
                    var medicament =
                        await data.Medicaments.FirstOrDefaultAsync(m => m.IdMedicament == med.IdMedicament);
                    if (medicament is null)
                    {
                        throw new NotFoundException($"Medicament with id: {med.IdMedicament} not found");
                    }
                }

                // check czy nie ma tych lekow wiecej niz powinno byc
                if (count > 10)
                {
                    throw new Wrong($"Medicaments count is over 10");
                }
            }

            // check czy daty sie zgadzaja
            if (prescriptionData.DueDate < prescriptionData.Date)
            {
                throw new Wrong($"Due date is less than prescription date");
            }

            var doctor = await data.Doctors.FirstOrDefaultAsync(d => d.IdDoctor == prescriptionData.IdDoctor);
            if (doctor == null)
            {
                throw new NotFoundException($"Lekarz o ID {prescriptionData.IdDoctor} nie istnieje.");
            }

            var prescription = new Prescription
            {
                Date = prescriptionData.Date,
                DueDate = prescriptionData.DueDate,
                IdPatient = prescriptionData.Patient.IdPatient,
                IdDoctor = prescriptionData.IdDoctor,
                Patient = patient,
                Doctor = doctor,
                PrescriptionMedicaments = (prescriptionData.Medicaments ?? []).Select(m => new PrescriptionMedicament
                {
                    IdMedicament = m.IdMedicament,
                    Dose = m.Dose,
                    Details = m.Details
                }).ToList()
            };

            data.Prescriptions.Add(prescription);
            await data.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return new PrescriptionDto
            {
                IdPrescription = prescription.IdPrescription,
                Date = prescription.Date,
                DueDate = prescription.DueDate,
                Doctor = new DoctorDto
                {
                    IdDoctor = doctor.IdDoctor,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Email = doctor.Email
                },
                Medicaments = prescription.PrescriptionMedicaments.Select(m => new MedicamentDto
                {
                    IdMedicament = m.IdMedicament,
                    Name = m.Medicament.Name,
                    Description = m.Medicament.Description,
                    Type = m.Medicament.Type,
                    Dose = m.Dose,
                    Details = m.Details
                }).ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}