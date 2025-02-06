namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _segmentⲻnzParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._segmentⲻnz> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._segmentⲻnz>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._segmentⲻnz> Parse(IInput<char>? input)
            {
                var _pchar_1 = __GeneratedOdataV4.Parsers.Rules._pcharParser.Instance.Repeat(1, null).Parse(input);
if (!_pchar_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._segmentⲻnz)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._segmentⲻnz(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._pchar>(_pchar_1.Parsed)), _pchar_1.Remainder);
            }
        }
    }
    
}
