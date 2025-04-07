namespace CombinatorParsingV3
{
    using System;

    public interface IFuture<out T> //// TODO i'm not sure i like having an interface...
    {
        T Value { get; }
    }
    public sealed class Future<T> : IFuture<T>
    {
        private readonly Func<T> promise;

        private Optional<T> value;

        public Future(Func<T> promise)
        {
            this.promise = promise;

            this.value = default;
        }

        public T Value
        {
            get
            {
                if (!this.value.TryGetValue(out var value))
                {
                    value = this.promise();

                    this.value = value;
                }

                return value;
            }
        }
    }

    public static class Future
    {
        public static Future<T> Create<T>(Func<T> promise)
        {
            return new Future<T>(promise);
        }
    }

    public static class FutureExtensions
    {
        public static IFuture<TResult> Select<TValue, TResult>(this IFuture<TValue> future, Func<TValue, TResult> selector)
        {
            return new Future<TResult>(() => selector(future.Value));
        }
    }
}
