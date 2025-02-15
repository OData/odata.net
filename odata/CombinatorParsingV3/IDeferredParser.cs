using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

namespace CombinatorParsingV3
{
    public interface IDeferredParser<TToken, TRealizedAstNode, TDefferedAstNode> where TDefferedAstNode : IDeferredAstNode<TToken, TRealizedAstNode>
    {
        TDefferedAstNode Parse(IInput<TToken> input);
    }

    public interface IDeferredAstNode<TToken, TRealizedAstNode>
    {
        IOutput<TToken, TRealizedAstNode> Realize();
    }

    public interface IDeferredOutput<TRealizedAstNode>
    {
        bool Success { get; }

        TRealizedAstNode Parsed { get; }
    }

    public sealed class DeferredOutput<TRealizedAstNode> : IDeferredOutput<TRealizedAstNode>
    {
        public DeferredOutput(bool success, TRealizedAstNode parsed)
        {
            Success = success;
            Parsed = parsed;
        }

        public bool Success { get; }

        public TRealizedAstNode Parsed { get; }
    }

    public interface IDeferredOutput2<TToken>
    {
        bool Success { get; }

        IInput<TToken> Remainder { get; }
    }

    public sealed class DeferredOutput2<TToken> : IDeferredOutput2<TToken>
    {
        public DeferredOutput2(bool success, IInput<TToken> remainder)
        {
            Success = success;
            Remainder = remainder;
        }

        public bool Success { get; }

        public IInput<TToken> Remainder { get; }
    }

    public static class DeferredOutput2
    {
        public static DeferredOutput2<TToken> FromValue<TToken>(this IInput<TToken> input)
        {
            return new DeferredOutput2<TToken>(true, input);
        }
    }
}
