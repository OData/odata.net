namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻrootlessParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻrootless> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻrootless>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻrootless> Parse(IInput<char>? input)
            {
                var _segmentⲻnz_1 = __GeneratedOdataV3.Parsers.Rules._segmentⲻnzParser.Instance.Parse(input);
if (!_segmentⲻnz_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pathⲻrootless)!, input);
}

var _Ⲥʺx2Fʺ_segmentↃ_1 = Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many().Parse(_segmentⲻnz_1.Remainder);
if (!_Ⲥʺx2Fʺ_segmentↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pathⲻrootless)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pathⲻrootless(_segmentⲻnz_1.Parsed,  _Ⲥʺx2Fʺ_segmentↃ_1.Parsed), _Ⲥʺx2Fʺ_segmentↃ_1.Remainder);
            }
        }
    }
    
}
