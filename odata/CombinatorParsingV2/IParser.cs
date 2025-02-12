using __GeneratedOdata.CstNodes.Inners;
using System;
using System.ComponentModel;

namespace CombinatorParsingV2
{
    //// TODO covariance and contravariance

    public interface IParser<TToken, out TParsed>
    {
        IOutput<TToken, TParsed> Parse(IInput<TToken>? input); //// TODO would it make sense to have a TInput and a IIndex? that way the index can be as small as an int, so there's no trade-off with using a struct? iindex would have a `TToken current(TInput)` method or something
    }

    public interface IInput<out TToken> //// TODO make a struct an use `in` parameter for `iparser.parse` method?
    {
        TToken Current { get; }

        IInput<TToken>? Next();
    }

    public interface IOutput<out TToken, out TParsed> //// TODO make a struct and use `in` parameter whereever applicable?
    {
        bool Success { get; }

        public TParsed Parsed { get; }

        IInput<TToken>? Remainder { get; }
    }


    public interface IParser2<TToken, TParsed>
    {
        Output2<TToken, TParsed> Parse2(IInput<TToken>? input);
    }

    public interface IOutput2<out TToken, TParsed> //// TODO make a struct and use `in` parameter whereever applicable?
    {
        bool Success { get; }

        IInput<TToken>? Remainder { get; }

        TParsed Parsed { get; }
    }

    public sealed class Output2<TToken, TParsed> : IOutput2<TToken, TParsed>
    {
        public Output2(bool success, TParsed parsed, IInput<TToken>? remainder)
        {
            Success = success;
            Remainder = remainder;
            this.Parsed = parsed;
        }

        public bool Success { get; }

        public IInput<TToken>? Remainder { get; }

        public TParsed Parsed { get; }
    }

    public struct NewNullable<T>
    {
        private readonly T value;

        public NewNullable(T value)
        {
            this.value = value;

            this.HasValue = true;
        }

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!this.HasValue)
                {
                    throw new InvalidOperationException("tODO");
                }

                return this.value;
            }
        }
    }

    public struct Nothing
    {
    }

    public interface IFuture<TOutput>
    {
        TOutput Value { get; }

        IFuture<TContinued> ContinueWith<TContinued>(Func<TOutput, TContinued> promise);
    }

    public sealed class Future<TInput, TOutput> : IFuture<TOutput> //// TODO can you remove the inheritance and make these ref structs? (also make newnullable a ref struct)
    {
        private readonly Func<TInput, TOutput> promise;
        private readonly IFuture<TInput> dependencies;

        private NewNullable<TOutput> value;

        public Future(TOutput value)
        {
            this.value = new NewNullable<TOutput>(value);
        }

        public Future(Func<TInput, TOutput> promise, IFuture<TInput> dependencies)
        {
            this.promise = promise;
            this.dependencies = dependencies;

            this.value = new NewNullable<TOutput>();
        }

        public TOutput Value
        {
            get
            {
                if (!this.value.HasValue)
                {
                    var input = this.dependencies.Value;

                    this.value = new NewNullable<TOutput>(this.promise(input));
                }

                return this.value.Value;
            }
        }

        public IFuture<TContinued> ContinueWith<TContinued>(Func<TOutput, TContinued> promise)
        {
            return new Future<TOutput, TContinued>(promise, this);
        }

        public static Future<Nothing, Nothing> Empty { get; } = new Future<Nothing, Nothing>(new Nothing());
    }

    public abstract class Future
    {
        private readonly NewNullable<Future> other;

        private Future()
        {
            this.other = new NewNullable<Future>();
        }

        public Future(Future other)
        {
            this.other = new NewNullable<Future>(other);
        }

        public void Wait()
        {
            if (!other.HasValue)
            {
                return;
            }

            other.Value.Wait();

            WaitImpl();
        }

        protected abstract void WaitImpl();

        public static Future Empty { get; } = new EmptyFuture();

        private sealed class EmptyFuture : Future
        {
            protected override void WaitImpl()
            {
            }
        }
    }
}
