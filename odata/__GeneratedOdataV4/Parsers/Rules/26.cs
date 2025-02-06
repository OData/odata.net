namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ordinalIndexParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ordinalIndex> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._ordinalIndex>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._ordinalIndex> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ordinalIndex)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_ʺx2Fʺ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ordinalIndex)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._ordinalIndex(_ʺx2Fʺ_1.Parsed, new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
