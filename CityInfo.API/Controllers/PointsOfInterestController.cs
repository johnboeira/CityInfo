using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId}/pointsofinterest")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDTO>> GetPointsOfInterest(int cityId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        if (city == null)
            return NotFound();

        return Ok(city.PointsOfInterest);
    }

    [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDTO> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointOfInterestId);
        if (pointOfInterest == null)
            return NotFound();

        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDTO> CreatePointOfInterest([FromRoute] int cityId, [FromBody] PointOfInterestCreationDTO pointOfInterestCreationDTO)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city is null)
            return NotFound();

        var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

        var finalPointOfInterest = new PointOfInterestDTO()
        {
            Id = ++maxPointOfInterestId,
            Name = pointOfInterestCreationDTO.Name,
            Description = pointOfInterestCreationDTO.Description,
        };

        city.PointsOfInterest.Add(finalPointOfInterest);

        return Created();

        //return CreatedAtRoute("GetPointOfInterest", new
        //{
        //    cityId = cityId,
        //    pointOfInterest = finalPointOfInterest.Id
        //},
        //finalPointOfInterest
        //);
    }

    [HttpPut("{pointofinterestid}")]
    public ActionResult Update([FromRoute] int cityId, [FromRoute] int pointofinterestid, PointOfInterestForUpdateDTO pointOfInterest)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var pointOfInterestCallBack = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
        if (pointOfInterestCallBack == null)
            return NotFound();

        pointOfInterestCallBack.Name = pointOfInterest.Name;
        pointOfInterestCallBack.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointofinterestid}")]
    public ActionResult PartiallyUpdatePointOfInterest([FromRoute] int cityId, [FromRoute] int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDTO> pacthDocument)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        if (city == null)
            return NotFound();

        var pointOfInterestCallBack = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
        if (pointOfInterestCallBack == null)
            return NotFound();

        var pointOfInterestToPatch = new PointOfInterestForUpdateDTO()
        {
            Name = pointOfInterestCallBack.Name,
            Description = pointOfInterestCallBack.Description,
        };

        pacthDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if(!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        if(!TryValidateModel(pointOfInterestToPatch)){
            return BadRequest(ModelState);
        }

        pointOfInterestCallBack.Name = pointOfInterestToPatch.Name;
        pointOfInterestCallBack.Description = pointOfInterestToPatch.Description;

        return NoContent();
    }
}