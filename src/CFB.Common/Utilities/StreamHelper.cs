using Newtonsoft.Json;
using System.IO;
using System.Text;
using Formatting = Newtonsoft.Json.Formatting;

namespace CFB.Common.Utilities
{
    public static class StreamHelper
    {
        private static readonly JsonSerializer _jsonSerilizer;

        static StreamHelper()
        {
            _jsonSerilizer = new JsonSerializer();
        }
        public static T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)(stream);
                }

                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return _jsonSerilizer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }

        public static Stream ToStream<T>(T input)
        {
            var memoryStream = new MemoryStream();

            using (var streamWriter = new StreamWriter(memoryStream, encoding: Encoding.Default, bufferSize: 1024, leaveOpen: true))
            {
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    jsonTextWriter.Formatting = Formatting.None;

                    _jsonSerilizer.Serialize(jsonTextWriter, input);
                    jsonTextWriter.Flush();
                    streamWriter.Flush();
                }
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
