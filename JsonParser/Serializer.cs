using System.Text;
using JsonParser.Models;

namespace JsonParser;

public static class Serializer
{
    private const string Null = "null";
    public static string Serialize(dynamic value)
    {
        if (value == null) return Null;
        if (value.GetType() == typeof(JObject))
        {
            return SerializeObject(value);
        }
        if (value.GetType() == typeof(dynamic[]))
        {
            return SerializeArray(value);
        }
        if (value is string)
        {
            return $"\"{value}\"";
        }
        return value.ToString();
    }
    
    private static string SerializeObject(JObject @object)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        int i = 0;
        foreach (var property in @object.Properties)
        {
            sb.Append($"\"{property.Key}\": {Serialize(property.Value)}");
            if ((@object.Properties.Count - 1) > i) sb.Append(',');
            i++;
        }
        sb.Append('}');
        return sb.ToString();
    }
    private static string SerializeArray(dynamic[] @array)
    {
        return $"[{string.Join(',', @array.Select(Serialize))}]";
    }
}