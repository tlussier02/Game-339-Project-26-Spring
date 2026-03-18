using System.Text;
using Game339.Shared.Diagnostics;

namespace Game339.Shared.Services.Implementation;

public class StringService : IStringService
{
    private readonly IGameLog _log;

    public StringService(IGameLog log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public string Reverse(string input)
    {
        var output = new string(input.Reverse().ToArray());
        _log.Info($"{nameof(StringService)}.{nameof(Reverse)} - {nameof(input)}: {input} - {nameof(output)}: {output}");
        return output;
    }

    public string ReverseWords(string input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        _log.Info("Reversing words.");

        var builder = new StringBuilder(input.Length);
        var token = new StringBuilder();
        var isWhitespace = false;

        foreach (var character in input)
        {
            var characterIsWhitespace = char.IsWhiteSpace(character);
            if (token.Length == 0)
            {
                isWhitespace = characterIsWhitespace;
            }

            if (characterIsWhitespace == isWhitespace)
            {
                token.Append(character);
                continue;
            }

            AppendToken(builder, token, isWhitespace);
            token.Clear();
            token.Append(character);
            isWhitespace = characterIsWhitespace;
        }

        AppendToken(builder, token, isWhitespace);
        return builder.ToString();
    }

    private static void AppendToken(StringBuilder builder, StringBuilder token, bool isWhitespace)
    {
        if (token.Length == 0)
        {
            return;
        }

        if (isWhitespace)
        {
            builder.Append(token);
            return;
        }

        for (var index = token.Length - 1; index >= 0; index--)
        {
            builder.Append(token[index]);
        }
    }
}
