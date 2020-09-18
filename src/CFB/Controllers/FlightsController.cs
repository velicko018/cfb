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
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [AllowAnonymous]
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<object> Search(FlightSearchDto flightSearchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var flights = await _flightService.SearchAsync(flightSearchDto);

            return Ok(flights);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFlights([FromQuery] PaginationParameters paginationParameters)
        {
            var flights = await _flightService.GetFlightsAsync(paginationParameters);

            if (flights is null)
            {
                return NotFound();
            }

            return Ok(flights);
        }

        [HttpGet("{flightId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFlight(Guid flightId)
        {
            var flight = await _flightService.GetFlightAsync(flightId);

            if (flight is null)
            {
                return NotFound();
            }

            return Ok(flight);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFlight(CreateFlightDto createFlightDto)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _flightService.CreateFlightAsync(createFlightDto);

            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return Ok();
        }

        [HttpDelete("{flightId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFlight(Guid flightId)
        {
            var result = await _flightService.DeleteFlightAsync(flightId);

            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return NoContent();
        }

        [HttpPut("{flightId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFlight(Guid flightId, UpdateFlightDto updateFlightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _flightService.UpdateFlightAsync(flightId, updateFlightDto);
        
            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, try again later.");
            }

            return Ok();
        }
    }
}
