namespace CombinatorParsingV3
{
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
