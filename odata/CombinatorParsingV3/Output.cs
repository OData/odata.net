namespace CombinatorParsingV3
{
    public static class Output
    {
        public static Output<TToken, TParsed> Create<TToken, TParsed>(bool success, TParsed parsed, IInput<TToken>? remainder)
        {
            return new Output<TToken, TParsed>(success, parsed, remainder);
        }
    }
}
