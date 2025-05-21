using Microsoft.AspNetCore.Mvc;
using Pharmacy.Exceptions;
using Pharmacy.Models.DTOs;
using Pharmacy.Services;

namespace Pharmacy.Controllers;

[ApiController]
[Route("[controller]")]
public class PrescriptionsController(IDbService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPrescriptionDetails([FromRoute] int id)
    {
        try
        {
            return Ok(await service.GetPrescriptionInfoAsync(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionCreateDto prescriptionData)
    {
        try
        {
            var prescription = await service.CreatePrescriptionAsync(prescriptionData);
            return CreatedAtAction(nameof(GetPrescriptionDetails), new { IdPrescription = prescription.IdPrescription },
                prescription);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Wrong w)
        {
            return BadRequest(w.Message);
        }
    }
}