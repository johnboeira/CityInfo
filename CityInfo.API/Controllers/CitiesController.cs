using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<CityDTO>> GetCities()
    {
        return Ok(CitiesDataStore.Current.Cities);
    }

    [HttpGet("{id}")]
    public ActionResult<CityDTO> GetCity(int id)
    {
        var cityCallBack = CitiesDataStore.Current.Cities
            .FirstOrDefault(c => c.Id == id);

        if (cityCallBack is null)
            return NotFound();

        return Ok(cityCallBack);
    }
}