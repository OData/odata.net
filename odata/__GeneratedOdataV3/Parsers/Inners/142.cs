namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_orderbyItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem)!, input);
}

var _orderbyItem_1 = __GeneratedOdataV3.Parsers.Rules._orderbyItemParser.Instance.Parse(_COMMA_1.Remainder);
if (!_orderbyItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_orderbyItem(_COMMA_1.Parsed, _orderbyItem_1.Parsed), _orderbyItem_1.Remainder);
            }
        }
    }
    
}
