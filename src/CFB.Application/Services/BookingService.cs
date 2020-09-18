using CFB.Application.Dispatchers;
using CFB.Application.Extensions;
using CFB.Common.DTOs;
using CFB.Domain.Commands;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CFB.Application.Models;

namespace CFB.Application.Services
{
    public interface IBookingService
    {
        Task<Result> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingsDto> GetBookingsAsync(PaginationParameters paginationParameters);
        Task<BookingsDto> GetBookingsByUserIdAsync(Guid userId);
    }

    public class BookingService : IBookingService
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly QueryDispatcher _queryDispatcher;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookingService(CommandDispatcher commandDispatcher, QueryDispatcher queryDispatcher, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            var user = _httpContextAccessor.HttpContext.User as ClaimsPrincipal;
            var createBookingCommand = new CreateBookingCommand
            {
                FlightIds = createBookingDto.FlightIds,
                Passangers = createBookingDto.Passangers,
                OwnerId = user.GetUserId()
            };

            var result = await _commandDispatcher.Dispatch(createBookingCommand);

            if (result.IsSuccess)
            {
                _memoryCache.TryGetValue(CacheKeys.Bookings, out long numberOfBookings);
                _memoryCache.Set(CacheKeys.Bookings, ++numberOfBookings);
            }

            return result;
        }

        public async Task<BookingsDto> GetBookingsAsync(PaginationParameters paginationParameters)
        {
            var getBookingsQuery = new GetBookingsQuery
            {
                RangeFrom = paginationParameters.PageIndex * paginationParameters.PageSize,
                RangeTo = paginationParameters.PageIndex * paginationParameters.PageSize + paginationParameters.PageSize
            };

            var bookings = await _queryDispatcher.Dispatch(getBookingsQuery);

            _memoryCache.TryGetValue(CacheKeys.Bookings, out long total);

            return new BookingsDto
            {
                Bookings = bookings,
                Total = total
            };
        }

        public async Task<BookingsDto> GetBookingsByUserIdAsync(Guid userId)
        {
            var getBookingsQuery = new GetBookingsByUserIdQuery
            {
                UserId = userId
            };

            var bookings = await _queryDispatcher.Dispatch(getBookingsQuery);

            return new BookingsDto
            {
                Bookings = bookings,
                Total = bookings.Count()
            };
        }
    }
}
