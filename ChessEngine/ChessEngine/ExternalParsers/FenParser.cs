#nullable enable
namespace ChessEngine.ExternalParsers;

public class FenParser
{
    private readonly string _source;

    public FenParser(string source)
    {
        _source = source;
    }

    public bool Parse(out string message, out string contents)
    {
        message = "";
        contents = "";



        return true;
    }
}