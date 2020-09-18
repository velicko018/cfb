using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CFB.Infrastructure.Logging
{
    class Log
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public string Message { get; set; }
        public string Exception { get; set; }
        public LogLevel Level { get; set; }
        public string ApplicationName { get; set; }
        public string Environment { get; set; }
    }
}
