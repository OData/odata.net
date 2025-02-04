namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_expandItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem)!, input);
}

var _expandItem_1 = __GeneratedOdataV3.Parsers.Rules._expandItemParser.Instance.Parse(_COMMA_1.Remainder);
if (!_expandItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_expandItem(_COMMA_1.Parsed, _expandItem_1.Parsed), _expandItem_1.Remainder);
            }
        }
    }
    
}
