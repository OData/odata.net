namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._segment> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._segment>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._segment> Parse(IInput<char>? input)
            {
                var _pchar_1 = __GeneratedOdataV4.Parsers.Rules._pcharParser.Instance.Many().Parse(input);
if (!_pchar_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._segment)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._segment(_pchar_1.Parsed), _pchar_1.Remainder);
            }
        }
    }
    
}
