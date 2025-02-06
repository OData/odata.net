namespace CombinatorParsingV3
{
    public sealed class Output<TToken, TParsed> : IOutput<TToken, TParsed>
    {
        public Output(bool success, TParsed parsed, IInput<TToken>? remainder)
        {
            this.Success = success;
            this.Parsed = parsed;
            this.Remainder = remainder;
        }

        public bool Success { get; }

        public TParsed Parsed { get; }

        public IInput<TToken>? Remainder { get; }
    }
}
