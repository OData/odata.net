namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_computeItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem)!, input);
}

var _computeItem_1 = __GeneratedOdataV3.Parsers.Rules._computeItemParser.Instance.Parse(_COMMA_1.Remainder);
if (!_computeItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_computeItem(_COMMA_1.Parsed,  _computeItem_1.Parsed), _computeItem_1.Remainder);
            }
        }
    }
    
}
