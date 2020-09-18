using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gremlin.Net.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CFB.Infrastructure.Persistence.CosmosGremlinClient.Models
{
    public class GremlinResult
    {
        private readonly ResultSet<JToken> _resultSet;

        public int StatusCode => (int)(long)_resultSet.StatusAttributes["x-ms-status-code"];
        public double TotalRequestCharge => (double)_resultSet.StatusAttributes["x-ms-total-request-charge"];

        public GremlinResult(ResultSet<JToken> resultSet)
        {
            _resultSet = resultSet;
        }

        public GremlinResult<T> ToObject<T>() where T : Vertex
        {
            return new GremlinResult<T>(_resultSet);
        }

        public GremlinResult<T> ToVEObject<T>() where T : Journey
        {
            return new GremlinResult<T>(_resultSet);
        }
    }

    public class GremlinResult<T> : GremlinResult, IEnumerable<T> 
        where T : class
    {
        public IReadOnlyCollection<T> Result { get; }

        public GremlinResult(ResultSet<JToken> resultSet)
            : base(resultSet)
        {
            Result = resultSet
                .Select(token => token.ToObject<IReadOnlyCollection<T>>(new JsonSerializer()))
                .First();
        }

        public List<T> ToList<T>() 
            where T : class
        {
            return Result.Select(t => (t as Vertex).ToObject<T>()).ToList();
        }

        public List<List<T>> ToVEList<T>() where T : class
        {
            return Result
                .Select(p => (p as Journey).Flights.Select(f => f.ToObject<T>()).ToList())
                .ToList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Result.GetEnumerator();
        }
    }
}
    