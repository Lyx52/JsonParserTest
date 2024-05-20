using JsonParser.Models;

namespace JsonParser;

public static class Parser
{
    public static dynamic ParseJson(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        var lexer = new Lexer(fs);
        var tokens = new Queue<Token>(lexer.ReadTokens());
        var token = tokens.Dequeue();
        return Parse(token, tokens);
    }

    public static object Parse(Token token, Queue<Token> tokens)
    {
        switch (token.Type)
        {
            case TokenType.ArrayOpen: return ParseArray(tokens);
            case TokenType.ObjectOpen: return ParseObject(tokens);
            case TokenType.Numeric: return ParseNumeric(token);
            case TokenType.String: return token.Value ?? string.Empty;
            case TokenType.Null: return null;
            default: throw new Exception("Invalid json");
        }
    }
    
    public static object ParseNumeric(Token token)
    {
        if (token.Value is null) return null;
        if (token.Value.Contains('.') && float.TryParse(token.Value, out var floatValue)) return floatValue;
        if (int.TryParse(token.Value, out var intValue)) return intValue;
        throw new Exception("Cant parse numeric value!");
    }
    public static dynamic[] ParseArray(Queue<Token> tokens)
    {
        var items = new List<dynamic>();
        var current = tokens.Dequeue();
        while (current.Type != TokenType.ArrayClose)
        {
            if (items.Count == 0)
            {
                items.Add(Parse(current, tokens));    
            }
            else
            {
                if (current.Type != TokenType.PropertySeparator)
                    throw new Exception("Expected comma as array item separator!");
                current = tokens.Dequeue();  
                items.Add(Parse(current, tokens));  
            }
            current = tokens.Dequeue(); 
            
        }
        return items.ToArray();
    }
    public static JObject ParseObject(Queue<Token> tokens)
    {
        var @object = new JObject();
        var current = tokens.Dequeue();
        string name = string.Empty;
        while (current.Type != TokenType.ObjectClose)
        {

            if (current.Type == TokenType.String && string.IsNullOrEmpty(name))
            {
                name = current.Value!;
            }

            if (current.Type == TokenType.KeyValueSeparator)
            {
                current = tokens.Dequeue();
                @object[name] = Parse(current, tokens);
                name = string.Empty;
            }
            
            current = tokens.Dequeue();
        }
        return @object;
    }
}