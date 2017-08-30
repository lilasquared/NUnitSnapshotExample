using System;
using Newtonsoft.Json;

namespace NUnitSnapshotExample
{
    public class JsonSerializer : ISnapshotSerializer
    {
        public static JsonSerializer Default = new JsonSerializer();

        public String Serialize<T>(T source)
        {
            return JsonConvert.SerializeObject(source, Formatting.Indented);
        }
    }
}