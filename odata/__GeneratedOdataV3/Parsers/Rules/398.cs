namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻabemptyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty> Parse(IInput<char>? input)
            {
                var _Ⲥʺx2Fʺ_segmentↃ_1 = Inners._Ⲥʺx2Fʺ_segmentↃParser.Instance.Many().Parse(input);
if (!_Ⲥʺx2Fʺ_segmentↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pathⲻabempty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pathⲻabempty(_Ⲥʺx2Fʺ_segmentↃ_1.Parsed), _Ⲥʺx2Fʺ_segmentↃ_1.Remainder);
            }
        }
    }
    
}
