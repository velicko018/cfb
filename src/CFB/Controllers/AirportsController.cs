using System;
using System.Threading.Tasks;
using CFB.Application.Services;
using CFB.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CFB.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AirportsController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportsController(IAirportService airportService)
        {
            _airportService = airportService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAirports([FromQuery] PaginationParameters paginationParameters)
        {
            var airports = await _airportService.GetAirportsAsync(paginationParameters);

            return Ok(airports);
        }


        [HttpGet("{airportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAirport(Guid airportId)
        {
            var airport = await _airportService.GetAirportAsync(airportId);

            if (airport is null)
            {
                return NotFound();
            }

            return Ok(airport);
        }

        [AllowAnonymous]
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(string state)
        {
            var airports = await _airportService.GetAirportsByStateAsync(state);

            return Ok(airports);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAirport(CreateAirportDto createAirportDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _airportService.CreateAirportAsync(createAirportDto);

            if(!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return Ok();
        }
           
        [HttpPut("{airportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAirport(Guid airportId, AirportDto updateAirportDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _airportService.UpdateAirportAsync(airportId, updateAirportDto);

            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return Ok();
        }
        [HttpDelete("{airportId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAirport(Guid airportId)
        {
            var result = await _airportService.DeleteAirportAsync(airportId);
            
            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return NoContent();
        }
    }
}