using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

namespace CombinatorParsingV3
{
    public interface IDeferredAstNode<TToken, TRealizedAstNode>
    {
        IOutput<TToken, TRealizedAstNode> Realize();
    }

    public interface IDeferredOutput<TToken> //// TODO probably this needs to be a difference name
    {
        bool Success { get; }

        IInput<TToken> Remainder { get; }
    }

    public sealed class DeferredOutput<TToken> : IDeferredOutput<TToken>
    {
        public DeferredOutput(bool success, IInput<TToken> remainder)
        {
            Success = success;
            Remainder = remainder;
        }

        public bool Success { get; }

        public IInput<TToken> Remainder { get; }
    }

    public static class DeferredOutput
    {
        public static DeferredOutput<TToken> Create<TToken>(IInput<TToken> input)
        {
            return new DeferredOutput<TToken>(true, input);
        }

        public static DeferredOutput<TToken> Create<TToken, TParsed>(IOutput<TToken, TParsed> output)
        {
            return new DeferredOutput<TToken>(output.Success, output.Remainder);
        }

        public static Func<DeferredOutput<TToken>> ToPromise<TToken, TParsed>(Func<IOutput<TToken, TParsed>> realize)
        {
            return () =>
            {
                var output = realize();
                return new DeferredOutput<TToken>(output.Success, output.Remainder);
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

/*
public sealed class Slash<TMode> : IDeferredAstNode<char, Slash<ParseMode.Realized>> where TMode : ParseMode
        {
            private readonly Future<IDeferredOutput<char>> previouslyParsedOutput;

            private readonly Future<IOutput<char, Slash<ParseMode.Realized>>> cachedOutput;

            public Slash(Future<IDeferredOutput<char>> previouslyParsedOutput)
            {
                if (typeof(TMode) != typeof(ParseMode.Deferred))
                {
                    throw new ArgumentException("TODO");
                }

                this.previouslyParsedOutput = previouslyParsedOutput;

                this.cachedOutput = new Future<IOutput<char, Slash<ParseMode.Realized>>>(() => this.RealizeImpl());
            }

            private Slash(Future<IOutput<char, Slash<ParseMode.Realized>>> output)
            {
                this.cachedOutput = output;
            }

            public IOutput<char, Slash<ParseMode.Realized>> Realize()
            {
                return cachedOutput.Value;
            }

            private IOutput<char, Slash<ParseMode.Realized>> RealizeImpl()
            {
                var output = this.previouslyParsedOutput.Value;
                if (!output.Success)
                {
                    return new Output<char, Slash<ParseMode.Realized>>(false, default, output.Remainder);
                }

                var input = output.Remainder;

                if (input.Current == '/')
                {
                    return new Output<char, Slash<ParseMode.Realized>>(
                        true,
                        new Slash<ParseMode.Realized>(this.cachedOutput),
                        input.Next());
                }
                else
                {
                    return new Output<char, Slash<ParseMode.Realized>>(false, default, input);
                }
            }
        }
*/
}
