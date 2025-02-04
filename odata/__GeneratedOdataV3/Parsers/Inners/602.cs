namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ> Parse(IInput<char>? input)
            {
                var _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ_1 = __GeneratedOdataV3.Parsers.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺParser.Instance.Parse(input);
if (!_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ(_SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ_1.Parsed), _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ_1.Remainder);
            }
        }
    }
    
}
