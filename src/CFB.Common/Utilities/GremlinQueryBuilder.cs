using System;
using System.Text;

namespace CFB.Common.Utilities
{
    public class GremlinQueryBuilder
    {

        private StringBuilder _stringBuilder;

        public GremlinQueryBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        public string Build()
        {
            return _stringBuilder.ToString();
        }

        public GremlinQueryBuilder AddVertex(string label, bool concate = false)
        {
            if (concate)
            {
                _stringBuilder.Append($".addV('{label}')");
            }
            else
            {
                _stringBuilder.Append($"g.addV('{label}')");
            }

            return this;
        }

        public GremlinQueryBuilder VertexAppend(string id = "", bool concate = false)
        {
            if (concate)
            {
                _stringBuilder.Append($".V()");
            }
            else
            {
                _stringBuilder.Append($".V('{id}')");
            }

            return this;
        }

        public GremlinQueryBuilder From(string id)
        {
            _stringBuilder.Append($".from(g.V('{id}'))");

            return this;
        }

        public GremlinQueryBuilder Vertex(string id = "")
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                _stringBuilder.Append($"g.V()");
            }
            else
            {
                _stringBuilder.Append($"g.V('{id}')");
            }

            return this;
        }

        public GremlinQueryBuilder HasLabel(string label)
        {
            _stringBuilder.Append($".hasLabel('{label}')");

            return this;
        }

        public GremlinQueryBuilder Property(string key, object value)
        {
            _stringBuilder.Append($".property('{key}', '{value}')");

            return this;
        }

        public GremlinQueryBuilder Has(string key, string value)
        {
            _stringBuilder.Append($".has('{key}', '{value}')");

            return this;
        }

        public GremlinQueryBuilder Has(string key, string filter, string value)
        {
            _stringBuilder.Append($".has('{key}', {filter}('{value}'))");

            return this;
        }

        public GremlinQueryBuilder AddEdge(string label)
        {
            _stringBuilder.Append($".addE('{label}')");

            return this;
        }

        public GremlinQueryBuilder As(string alias)
        {
            _stringBuilder.Append($".as('{alias}')");

            return this;
        }

        public GremlinQueryBuilder To(Guid flightId)
        {
            _stringBuilder.Append($".to(g.V('{flightId}'))");

            return this;
        }

        public GremlinQueryBuilder Drop()
        {
            _stringBuilder.Append($".drop()");

            return this;
        }

        public GremlinQueryBuilder OutEdge(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _stringBuilder.Append(".outE()");
            }
            else
            {
                _stringBuilder.Append($".outE('{name}')");
            }

            return this;
        }

        public GremlinQueryBuilder InEdge(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _stringBuilder.Append(".inE()");
            }
            else
            {
                _stringBuilder.Append($".inE('{name}')");
            }

            return this;
        }

        public GremlinQueryBuilder Path()
        {
            _stringBuilder.Append(".path()");

            return this;
        }

        public GremlinQueryBuilder Range(int rangeFrom, int rangeTo)
        {
            _stringBuilder.Append($".range({rangeFrom}, {rangeTo})");

            return this;
        }

        public GremlinQueryBuilder RepeatOutSimplePath()
        {
            _stringBuilder.Append(".repeat(out().simplePath())");

            return this;
        }

        public GremlinQueryBuilder InVertex()
        {
            _stringBuilder.Append(".inV()");

            return this;
        }

        public GremlinQueryBuilder OutVertex()
        {
            _stringBuilder.Append(".outV()");

            return this;
        }

        public GremlinQueryBuilder UntilHasId(string to)
        {
            _stringBuilder.Append($".until(hasId('{to}'))");

            return this;
        }
    }
}
