namespace CombinatorParsingV3
{
    public interface IDeferredParser<TToken, TOutput, TNode> where TNode : IDeferredAstNode<TOutput>
    {
        TNode Parse(IInput<TToken> input);
    }

    public interface IDeferredAstNode<TOutput>
    {
        IDeferredOutput<TOutput> Realize();
    }

    public interface IDeferredOutput<TOutput>
    {
        bool Success { get; }

        TOutput Parsed { get; }
    }

    public sealed class DeferredOutput<TOutput> : IDeferredOutput<TOutput>
    {
        public DeferredOutput(bool success, TOutput parsed)
        {
            Success = success;
            Parsed = parsed;
        }

        public bool Success { get; }

        public TOutput Parsed { get; }
    }
}
