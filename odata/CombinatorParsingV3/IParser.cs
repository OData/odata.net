using __GeneratedOdata.CstNodes.Rules;

namespace CombinatorParsingV3
{
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
