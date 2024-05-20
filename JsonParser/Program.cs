using JsonParser.Models;

namespace JsonParser;


public class Program
{
    public static void Main()
    {
        var obj = Parser.ParseJson("test.json");
        var x = obj["test444"]["test123"];
        obj["test4"] = 333f;
        var data = Serializer.Serialize(obj);
    }
}