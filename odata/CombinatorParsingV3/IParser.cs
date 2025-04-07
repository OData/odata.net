namespace CombinatorParsingV3
{
    public interface IRealizationResult<out TToken, out TRealized> : IRealizationResult<TToken>
    {
        TRealized RealizedValue { get; }
    }

    public sealed class RealizationResult<TToken, TRealized> : IRealizationResult<TToken, TRealized>
    {
        public RealizationResult(bool success, TRealized realizedValue, ITokenStream<TToken>? remainingTokens)
        {
            Success = success;
            RealizedValue = realizedValue;
            RemainingTokens = remainingTokens;
        }

        public bool Success { get; }

        public TRealized RealizedValue { get; }

        public ITokenStream<TToken>? RemainingTokens { get; }
    }

    public interface ITokenStream<out TToken>
    {
        TToken Current { get; }

        ITokenStream<TToken>? Next();
    }

    public sealed class CharacterTokenStream : ITokenStream<char>
    {
        private readonly string input;
        private readonly int index;

        public CharacterTokenStream(string input)
            : this(input, 0)
        {
        }

        private CharacterTokenStream(string input, int index)
        {
            this.input = input;
            this.index = index;
        }

        public char Current
        {
            get
            {
                return this.input[this.index];
            }
        }

        public ITokenStream<char>? Next()
        {
            var newIndex = this.index + 1;
            if (newIndex >= this.input.Length)
            {
                return null;
            }

            return new CharacterTokenStream(this.input, newIndex);
        }
    }
    public interface IDeferredAstNode<out TToken, out TRealizedAstNode>
    {
        IRealizationResult<TToken, TRealizedAstNode> Realize();
    }

    public interface IFromRealizedable<out TDeferredAstNode>
    {
        TDeferredAstNode Convert();
    }

    public interface IRealizationResult<out TToken>
    {
        bool Success { get; }

        ITokenStream<TToken>? RemainingTokens { get; }
    }

    public sealed class RealizationResult<TToken> : IRealizationResult<TToken>
    {
        public RealizationResult(bool success, ITokenStream<TToken>? remainder)
        {
            Success = success;
            RemainingTokens = remainder;
        }

        public bool Success { get; }

        public ITokenStream<TToken>? RemainingTokens { get; }
    }

    public abstract class ParseMode
    {
        private ParseMode()
        {
        }

        public sealed class Deferred : ParseMode
        {
            private Deferred()
            {
            }
        }

        public sealed class Realized : ParseMode
        {
            private Realized()
            {
            }
        }
    }
}
