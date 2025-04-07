using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

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

    public interface IDeferredOutput<TToken> //// TODO probably this needs to be a difference name
    {
        bool Success { get; }

        ITokenStream<TToken> Remainder { get; }
    }

    public sealed class DeferredOutput<TToken> : IDeferredOutput<TToken>
    {
        public DeferredOutput(bool success, ITokenStream<TToken> remainder)
        {
            Success = success;
            Remainder = remainder;
        }

        public bool Success { get; }

        public ITokenStream<TToken> Remainder { get; }
    }

    public static class DeferredOutput
    {
        public static DeferredOutput<TToken> Create<TToken>(ITokenStream<TToken> input)
        {
            return new DeferredOutput<TToken>(true, input);
        }

        public static DeferredOutput<TToken> Create<TToken, TParsed>(IRealizationResult<TToken, TParsed> output)
        {
            return new DeferredOutput<TToken>(output.Success, output.RemainingTokens);
        }

        public static Func<DeferredOutput<TToken>> ToPromise<TToken, TParsed>(Func<IRealizationResult<TToken, TParsed>> realize)
        {
            return () =>
            {
                var output = realize();
                return new DeferredOutput<TToken>(output.Success, output.RemainingTokens);
            };
        }
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
