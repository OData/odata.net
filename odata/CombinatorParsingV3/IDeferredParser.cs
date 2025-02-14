using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CombinatorParsingV3
{
    public interface IDeferredParser<TToken, TRealizedAstNode, TDefferedAstNode> where TDefferedAstNode : IDeferredAstNode<TRealizedAstNode>
    {
        TDefferedAstNode Parse(IInput<TToken> input);
    }

    public interface IDeferredAstNode<TRealizedAstNode>
    {
        IDeferredOutput<TRealizedAstNode> Realize();
    }

    public interface IDeferredOutput<TRealizedAstNode>
    {
        bool Success { get; }

        TRealizedAstNode Parsed { get; }

        List<RegexParseError>
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
}
