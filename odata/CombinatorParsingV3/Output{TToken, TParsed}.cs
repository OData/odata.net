namespace CombinatorParsingV3
{
    public readonly ref struct Output<TToken, TInput, TParsed> : IOutput<TToken, TInput, TParsed> where TInput : IInput<TToken, TInput>, allows ref struct
    {
        public Output(bool success, TParsed parsed, bool hasRemainder, TInput remainder)
        {
            this.Success = success;
            this.Parsed = parsed;
            this.HasRemainder = hasRemainder;
            this.Remainder = remainder;
        }

        public bool Success { get; }

        public TParsed Parsed { get; }

        public bool HasRemainder { get; }

        public TInput Remainder { get; }
    }
}
