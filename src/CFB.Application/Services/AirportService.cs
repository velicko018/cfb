using CFB.Application.Dispatchers;
using CFB.Application.Models;
using CFB.Common.DTOs;
using CFB.Domain.Commands;
using CFB.Domain.Queries;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;

using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFB.Application.Services
{
    public interface IAirportService
    {
        Task<Result> CreateAirportAsync(CreateAirportDto createAirportDto);
        Task<AirportDto> GetAirportAsync(Guid airportId);
        Task<AirportsDto> GetAirportsAsync(PaginationParameters paginationParameters);
        Task<Result> DeleteAirportAsync(Guid airportId);
        Task<Result> UpdateAirportAsync(Guid airportId, AirportDto airportDto);
        Task<IEnumerable<AirportDto>> GetAirportsByStateAsync(string state);
    }

    public class AirportService : IAirportService
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly QueryDispatcher _queryDispatcher;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageService _storageService;

        public AirportService(CommandDispatcher commandDispatcher, QueryDispatcher queryDispatcher, IMemoryCache memoryCache, IStorageService storageService)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _memoryCache = memoryCache;
            _storageService = storageService;
        }

        public async Task<Result> CreateAirportAsync(CreateAirportDto createAirportDto)
        {
            var airportId = Guid.NewGuid();
            var createAirportCommand = new CreateAirportCommand
            {
                Id = airportId,
                Name = createAirportDto.Name,
                City = createAirportDto.City,
                State = createAirportDto.State,
                Latitude = createAirportDto.Latitude,
                Longitude = createAirportDto.Longitude,
                ICAO = createAirportDto.ICAO,
                IATA = createAirportDto.IATA
            };

            var result = await _commandDispatcher.Dispatch(createAirportCommand);

            if (result.IsSuccess)
            {
                _memoryCache.TryGetValue(CacheKeys.Airports, out long numberOfAirports);
                _memoryCache.Set(CacheKeys.Airports, ++numberOfAirports);

                if (createAirportDto.Image != null)
                {
                    await _storageService.UploadAsync(createAirportDto.Image, airportId.ToString());
                }
            }

            return result;
        }

        public async Task<Result> DeleteAirportAsync(Guid airportId)
        {
            var deleteAirportCommand = new DeleteAirportCommand
            {
                AirportId = airportId
            };

            var result =  await _commandDispatcher.Dispatch(deleteAirportCommand);

            if (result.IsSuccess)
            {
                await _storageService.DeleteAsync(airportId.ToString());
                _memoryCache.TryGetValue(CacheKeys.Airports, out long numberOfAirports);
                _memoryCache.Set(CacheKeys.Airports, --numberOfAirports);
            }

            return result;
        }

        public async Task<AirportDto> GetAirportAsync(Guid airportId)
        {
            var getAirportQuery = new GetAirportQuery
            {
                Id = airportId.ToString()
            };

            var airport = await _queryDispatcher.Dispatch(getAirportQuery);

            return airport;
        }

        public async Task<IEnumerable<AirportDto>> GetAirportsByStateAsync(string state)
        {
            var getAirportsByStateQuery = new GetAirportsByStateQuery
            {
                State = state
            };

            var airports = await _queryDispatcher.Dispatch(getAirportsByStateQuery);

            return airports;
        }

        public async Task<AirportsDto> GetAirportsAsync(PaginationParameters paginationParameters)
        {
            var getAirportsQuery = new GetAirportsQuery
            {
                RangeFrom = paginationParameters.PageIndex * paginationParameters.PageSize,
                RangeTo = paginationParameters.PageIndex * paginationParameters.PageSize + paginationParameters.PageSize
            };

            var airports = await _queryDispatcher.Dispatch(getAirportsQuery);

            if (!airports.Any())
            {
                return null;
            }
            
            _memoryCache.TryGetValue(CacheKeys.Airports, out long total);

            return new AirportsDto
            {
                Airports = airports,
                Total = total
            };
        }

        public async Task<Result> UpdateAirportAsync(Guid airportId, AirportDto airportDto)
        {
            var updateAirportCommand = new UpdateAirportCommand
            {
                Id = airportId,
                Name = airportDto.Name,
                City = airportDto.City,
                State = airportDto.State,
                Latitude = airportDto.Latitude,
                Longitude = airportDto.Longitude,
                ICAO = airportDto.ICAO,
                IATA = airportDto.IATA
            };

            var result = await _commandDispatcher.Dispatch(updateAirportCommand);

            if (result.IsSuccess && airportDto.Image != null)
            {
                await _storageService.UploadAsync(airportDto.Image, airportId.ToString());
            }

            return result;
        }
    }
}