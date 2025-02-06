using __GeneratedOdata.CstNodes.Rules;

namespace CombinatorParsingV3
{
    //// TODO adding to the call stack increased the runtime; there could be a lot of reasons for this:
    //// 1. new node object allocations that were previously singletons
    //// 2. copy semantics of ref struct (`in` mitigated this a bit, `ref` mitigated it as well, but a little worse than in; it's not clear to me that the perf benefits of ref struct will remain as we go deeper in the call stack)
    //// 3. checking for success and remainder (you're removed this for now, but you need to add it back at some point)

    //// TODO covariance and contravariance

    public interface IParser<TToken, TInput, out TParsed, out TOutput> where TInput : IInput<TToken, TInput>, allows ref struct where TOutput : IOutput<TToken, TInput, TParsed>, allows ref struct
    {
         TOutput Parse(in TInput input);
    }

    public interface IInput<out TToken, out TInput> where TInput : IInput<TToken, TInput>, allows ref struct
    {
        TToken Current { get; }

        TInput Next(out bool success);
    }

    /// <summary>
    /// TODO make this a ref struct next
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TParsed"></typeparam>
    public interface IOutput<out TToken, out TInput, out TParsed> where TInput : IInput<TToken, TInput>, allows ref struct
    {
        bool Success { get; }

        TParsed Parsed { get; }

        bool HasRemainder { get; }

        TInput Remainder { get; }
    }
}
