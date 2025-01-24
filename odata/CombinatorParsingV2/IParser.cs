namespace CombinatorParsingV2
{
    //// TODO covariance and contravariance
    
    public interface IParser<TToken, out TParsed>
    {
        IOutput<TToken, TParsed> Parse(IInput<TToken> input);
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
}
