namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_segmentParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment)!, input);
}

var _segment_1 = __GeneratedOdataV4.Parsers.Rules._segmentParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_segment_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_segment(_ʺx2Fʺ_1.Parsed, _segment_1.Parsed), _segment_1.Remainder);
            }
        }
    }
    
}
