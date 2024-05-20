namespace JsonParser.Models;

public enum TokenType
{
    ObjectOpen,
    ObjectClose,
    ArrayOpen,
    ArrayClose,
    String,
    KeyValueSeparator,
    PropertySeparator,
    Numeric,
    Null,
    EoF
}