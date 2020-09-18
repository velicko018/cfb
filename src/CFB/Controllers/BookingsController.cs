using System;
using System.Threading.Tasks;
using CFB.Application.Services;
using CFB.Common.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CFB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookings([FromQuery] PaginationParameters paginationParameters)
        {
            var bookings = await _bookingService.GetBookingsAsync(paginationParameters);

            return Ok(bookings);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookingsByUserId(Guid userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);

            return Ok(bookings);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _bookingService.CreateBookingAsync(createBookingDto);

            if (!result.IsSuccess)
            {
                return BadRequest("Something went wrong. Please, contact technical support.");
            }

            return Ok();
        }
    }
}