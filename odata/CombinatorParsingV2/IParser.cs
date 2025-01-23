namespace CombinatorParsingV2
{
    //// TODO covariance and contravariance
    
    public interface IParser<TToken, out TParsed>
    {
        IOutput<TToken, TParsed> Parse(IInput<TToken> input);
    }

    public interface IInput<out TToken>
    {
        TToken Current { get; }

        IInput<TToken> Next();
    }

    public interface IOutput<out TToken, out TParsed>
    {
        bool Success { get; }

        public TParsed Parsed { get; }

        IInput<TToken> Remainder { get; }
    }
}
