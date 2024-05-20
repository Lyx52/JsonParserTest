using System.Text;

namespace JsonParser.Models;

public class JObject
{
    public Dictionary<string, dynamic> Properties;

    public dynamic this[string key]
    {
        get => Properties.ContainsKey(key) ? Properties[key] : throw new KeyNotFoundException($"Key {key} does not exist!");
        set => Properties[key] = value;
    }
    public JObject()
    {
        Properties = new Dictionary<string, dynamic>();
    }
}