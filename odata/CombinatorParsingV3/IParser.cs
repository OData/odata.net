using __GeneratedOdata.CstNodes.Rules;
using System.Collections.Generic;

namespace CombinatorParsingV3
{
    //// TODO adding to the call stack increased the runtime; there could be a lot of reasons for this:
    //// 1. new node object allocations that were previously singletons
    //// 2. copy semantics of ref struct (`in` mitigated this a bit, `ref` mitigated it as well, but a little worse than in; it's not clear to me that the perf benefits of ref struct will remain as we go deeper in the call stack)
    //// 3. checking for success and remainder (you're removed this for now, but you need to add it back at some point)
    //// 
    //// you did try updating `parse` to have `tparsed` as an `out` parameter hoping to reduce the amount of copying needed when creating `output` instances, but it didn't seem to have much impact; it may be worth revisiting this in the future when the call stack is deeper and more instances need to be created; you might also consider having *all* of the values as `out` parameters so that you can always pass something through

    //// TODO instead of having a `remainder` property on `output`, can you have output tell the caller how much they need to advance? they constructor on `output` can't take `stringinput` as a reference, so this might be a way around that
    //// TODO add success checks back
    //// TODO add ors back

    //// TODO covariance and contravariance

    public interface IParser<TToken, TInput, TParsed>
        where TInput : IReadOnlyList<TToken> //// TODO use IInput here
    {
         TParsed Parse(TInput input, out int advancement);
    }

    public interface IInput<out TToken, out TInput> where TInput : IInput<TToken, TInput>, allows ref struct
    {
        TToken Current { get; }

        TInput Next(out bool success);
    }

    /// <summary>
    /// 
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
