namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _indexParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._index> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._index>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._index> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._index)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._index)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Repeat(1, null).Parse(_EQ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._index)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._index(_Ⲥʺx24x69x6Ex64x65x78ʺⳆʺx69x6Ex64x65x78ʺↃ_1.Parsed, _EQ_1.Parsed, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._DIGIT>(_DIGIT_1.Parsed)), _DIGIT_1.Remainder);
            }
        }
    }
    
}
