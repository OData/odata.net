namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx2Fʺ_segmentↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_segmentↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_segmentↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_segmentↃ> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_segment_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_segmentParser.Instance.Parse(input);
if (!_ʺx2Fʺ_segment_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_segmentↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Fʺ_segmentↃ(_ʺx2Fʺ_segment_1.Parsed), _ʺx2Fʺ_segment_1.Remainder);
            }
        }
    }
    
}
