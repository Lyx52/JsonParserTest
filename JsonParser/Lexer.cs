using System.Text;
using JsonParser.Models;

namespace JsonParser;

public class Lexer
{
    private Stream _stream;
    private List<Token> _tokens;
    private static readonly char[] _null = { 'n', 'u', 'l', 'l' };
    public Lexer(Stream stream)
    {
        _stream = stream;
        _tokens = new List<Token>();
    }
    public IEnumerable<Token> ReadTokens()
    {
        var currentByte = _stream.ReadByte();
        while (currentByte != -1)
        {
            if (IsNullKeyword(currentByte)) AddToken(TokenType.Null);
            switch (currentByte)
            {
                case '{': AddToken(TokenType.ObjectOpen); break;
                case '}': AddToken(TokenType.ObjectClose); break;
                case '[': AddToken(TokenType.ArrayOpen); break;
                case ']': AddToken(TokenType.ArrayClose); break;
                case ':': AddToken(TokenType.KeyValueSeparator); break;
                case ',': AddToken(TokenType.PropertySeparator); break;
                case '"': AddToken(TokenType.String, ReadString()); break;
                case '.':
                case '-': 
                case '0': 
                case '1': 
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9': 
                    AddToken(TokenType.Numeric, ReadNumeric(currentByte));
                    break;
                case '\n':
                case ' ':
                case '\t':
                case '\r':
                    break;
            }
            currentByte = _stream.ReadByte();
        }
        AddToken(TokenType.EoF);
        return _tokens;
    }

    private string ReadNumeric(int currentByte)
    {
        StringBuilder sb = new StringBuilder();
        bool isNegative = currentByte == '-';
        bool isZero =  currentByte == '0';
        bool isDecimal = false;
        
        while (IsDigit(currentByte) || currentByte == '.' || currentByte == '-')
        {
            if (!isDecimal && currentByte == '.')
            {
                isDecimal = true;
            } else if (currentByte == '.') 
                throw new Exception("Unexpected numeric value!");
            if (isZero && IsNonZeroDigit(currentByte)) 
                throw new Exception("Unexpected numeric value!");
            if (sb.Length == 1 && isNegative && isDecimal)
                throw new Exception("Unexpected numeric value!");
            
            sb.Append((char)currentByte);
            currentByte = _stream.ReadByte();
        }
        
        var result = sb.ToString();
        if (isNegative && sb.Length == 0) 
            throw new Exception("Unexpected numeric value!");
        if (result.Length > 0 && result[0] == '.') 
            throw new Exception("Unexpected numeric value!");
        _stream.Seek(-1, SeekOrigin.Current); // We consumed extra byte
        return sb.ToString();
    }

    private bool IsNullKeyword(int currentByte)
    {
        bool isNull = true;
        foreach (var chr in _null)
        {
            if (currentByte != chr)
            {
                isNull = false;
                break;
            }

            currentByte = _stream.ReadByte();
        }

        return isNull;
    }
    private bool IsDigit(int chr) => IsNonZeroDigit(chr) || chr == 0x30;
    private bool IsNonZeroDigit(int chr) => chr >= 0x31 && chr <= 0x39;
    private string ReadString()
    {
        StringBuilder sb = new StringBuilder();
        var currentByte = _stream.ReadByte();
        while (currentByte != -1 && currentByte != '"')
        {
            sb.Append((char)currentByte);
            currentByte = _stream.ReadByte();
        }

        if (currentByte == -1)
        {
            throw new EndOfStreamException("Unexpected end of string");
        }
        return sb.ToString();
    }
    public void AddToken(TokenType type, string? value = null)
    {
        _tokens.Add(new Token()
        {
            Value = value,
            Type = type
        });
    }
}