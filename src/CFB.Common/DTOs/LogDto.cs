using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CFB.Common.DTOs
{
    [BsonIgnoreExtraElements]
    public class LogDto
    {
        public ObjectId Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }

    public class LogsDto
    {
        public IEnumerable<LogDto> Logs { get; set; }
        public long Total { get; set; }
    }
}
