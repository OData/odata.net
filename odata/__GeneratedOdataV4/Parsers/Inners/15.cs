namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥsegmentⲻnz_ʺx2FʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃ> Parse(IInput<char>? input)
            {
                var _segmentⲻnz_ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._segmentⲻnz_ʺx2FʺParser.Instance.Parse(input);
if (!_segmentⲻnz_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃ(_segmentⲻnz_ʺx2Fʺ_1.Parsed), _segmentⲻnz_ʺx2Fʺ_1.Remainder);
            }
        }
    }
    
}
