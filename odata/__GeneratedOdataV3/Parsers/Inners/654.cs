namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ> Parse(IInput<char>? input)
            {
                var _segmentⲻnz_1 = __GeneratedOdataV3.Parsers.Rules._segmentⲻnzParser.Instance.Parse(input);
if (!_segmentⲻnz_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ)!, input);
}

var _Ⲥʺx2Fʺ_segmentↃ_1 = Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many().Parse(_segmentⲻnz_1.Remainder);
if (!_Ⲥʺx2Fʺ_segmentↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ(_segmentⲻnz_1.Parsed,  _Ⲥʺx2Fʺ_segmentↃ_1.Parsed), _Ⲥʺx2Fʺ_segmentↃ_1.Remainder);
            }
        }
    }
    
}
