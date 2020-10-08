using CFB.Application.Dispatchers;
using CFB.Application.Models;
using CFB.Common.DTOs;
using CFB.Domain.Commands;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CFB.Application.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<object>> SearchAsync(FlightSearchDto flightSearchDto);
        Task<FlightsDto> GetFlightsAsync(PaginationParameters paginationParameters);
        Task<Result> CreateFlightAsync(CreateFlightDto createFlightDto);
        Task<Result> DeleteFlightAsync(Guid flightId);
        Task<Result> UpdateFlightAsync(Guid flightId, UpdateFlightDto updateFlightDto);
        Task<FlightDto> GetFlightAsync(Guid flightId);
    }

    public class FlightService : IFlightService
    {
        private readonly QueryDispatcher _queryDispatcher;
        private readonly CommandDispatcher _commandDispatcher;
        private readonly IMemoryCache _memoryCache;

        public FlightService(QueryDispatcher queryDispatcher, CommandDispatcher commandDispatcher, IMemoryCache memoryCache)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _memoryCache = memoryCache;
        }

        public async Task<Result> CreateFlightAsync(CreateFlightDto createFlightDto)
        {
            var createFlightCommand = new CreateFlightCommand
            {
                OriginAirportInfo = createFlightDto.OriginAirportInfo,
                DestinationAirportInfo = createFlightDto.DestinationAirportInfo,
                From = createFlightDto.From,
                To = createFlightDto.To,
                Departure = createFlightDto.Departure,
                Duration = createFlightDto.Duration
            };

            var result = await _commandDispatcher.Dispatch(createFlightCommand);

            if (result.IsSuccess)
            {
                _memoryCache.TryGetValue(CacheKeys.Flights, out long numberOfFlights);
                _memoryCache.Set(CacheKeys.Flights, ++numberOfFlights);
            }

            return result;
        }

        public async Task<Result> DeleteFlightAsync(Guid flightId)
        {
            var deleteFlightCommand = new DeleteFlightCommand
            {
                FlightId = flightId
            };

            var result = await _commandDispatcher.Dispatch(deleteFlightCommand);

            if (result.IsSuccess)
            {
                _memoryCache.TryGetValue(CacheKeys.Flights, out long numberOfFlights);
                _memoryCache.Set(CacheKeys.Flights, --numberOfFlights);
            }

            return result;
        }

        public async Task<FlightDto> GetFlightAsync(Guid flightId)
        {
            var getFlightQuery = new GetFlightQuery
            {
                Id = flightId.ToString()
            };

            var flight = await _queryDispatcher.Dispatch(getFlightQuery);

            return flight;
        }

        public async Task<FlightsDto> GetFlightsAsync(PaginationParameters paginationParameters)
        {
            var getFlightsQuery = new GetFlightsQuery
            {
                RangeFrom = paginationParameters.PageIndex * paginationParameters.PageSize,
                RangeTo = paginationParameters.PageIndex * paginationParameters.PageSize + paginationParameters.PageSize
            };
            var flights = await _queryDispatcher.Dispatch(getFlightsQuery);

            _memoryCache.TryGetValue(CacheKeys.Flights, out long total);

            return new FlightsDto
            {
                Flights = flights,
                Total = total
            };
        }

        public async Task<IEnumerable<object>> SearchAsync(FlightSearchDto flightSearchDto)
        {
            var flightSearchQuery = new FlightSearchQuery
            {
                From = flightSearchDto.From,
                To = flightSearchDto.To,
                Departure = flightSearchDto.Departure,
                Return = flightSearchDto.Return,
                NumberOfStops = flightSearchDto.NumberOfStops,
                FirstStop = flightSearchDto.FirstStop,
                SecondStop = flightSearchDto.SecondStop
            };
            var flights = await _queryDispatcher.Dispatch(flightSearchQuery);

            return flights;
        }

        public async Task<Result> UpdateFlightAsync(Guid flightId, UpdateFlightDto updateFlightDto)
        {
            var updateFlightCommand = new UpdateFlightCommand
            {
                Id = flightId,
                Departure = updateFlightDto.Departure,
                Duration = updateFlightDto.Duration
            };

            var result = await _commandDispatcher.Dispatch(updateFlightCommand);

            return result;
        }
    }
}
