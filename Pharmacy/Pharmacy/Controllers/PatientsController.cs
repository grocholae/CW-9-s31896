using Microsoft.AspNetCore.Mvc;
using Pharmacy.Exceptions;
using Pharmacy.Models.DTOs;
using Pharmacy.Services;

namespace Pharmacy.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController(IDbService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientDetails([FromRoute] int id)
    {
        try
        {
            return Ok(await service.GetPatientInfoAsync(id));
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}