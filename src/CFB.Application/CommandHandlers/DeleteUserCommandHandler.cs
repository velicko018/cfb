﻿using CFB.Application.Models;
using CFB.Common.Utilities;
using CFB.Domain.Commands;
using CFB.Infrastructure.Persistence.CosmosGremlinClient;
using Gremlin.Net.Driver;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CFB.Application.CommandHandlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IGremlinClient _gremlinClient;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IGremlinClient gremlinClient, ILogger<DeleteUserCommandHandler> logger) 
        {
            _gremlinClient = gremlinClient;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteUserCommand deleteUserCommand)
        {
            try
            {
                var query = new GremlinQueryBuilder()
                    .Vertex(deleteUserCommand.UserId.ToString())
                    .Drop()
                    .Build();

                var result = await _gremlinClient.SubmitAsyncQuery(query);

                if (result.StatusCode >= 200 && result.StatusCode <= 299)
                {
                    return Result.Success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return Result.Failure;
        }
    }
}
