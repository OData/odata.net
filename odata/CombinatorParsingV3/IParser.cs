using CombinatorParsingV1;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CombinatorParsingV3
{
    public interface IParser<TToken, TOutput>
    {
        IOutput<TToken, TOutput> Parse(IInput<TToken> input);
    }

    public interface IOutput<TToken, TOutput>
    {
        bool Success { get; }

        TOutput Parsed { get; }

        IInput<TToken> Remainder { get; }

        List<RegexParseError>
    }

    public sealed class Output<TToken, TOutput> : IOutput<TToken, TOutput>
    {
        public Output(bool success, TOutput parsed, IInput<TToken> remainder)
        {
            Success = success;
            Parsed = parsed;
            Remainder = remainder;
        }

        public bool Success { get; }

        public TOutput Parsed { get; }

        public IInput<TToken> Remainder { get; }
    }

    public interface IInput<TToken>
    {
        TToken Current { get; }

        IInput<TToken> Next();
    }
}
