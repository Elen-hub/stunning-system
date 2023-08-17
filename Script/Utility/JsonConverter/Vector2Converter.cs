using System;
using UnityEngine;
using Newtonsoft.Json;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var t = serializer.Deserialize(reader);
        var iv = JsonConvert.DeserializeObject<Vector2>(t.ToString());
        return iv;
    }
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        Vector2 v = value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WriteEndObject();
    }
}
