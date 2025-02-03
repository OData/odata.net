namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_selectItemParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem)!, input);
}

var _selectItem_1 = __GeneratedOdataV3.Parsers.Rules._selectItemParser.Instance.Parse(_COMMA_1.Remainder);
if (!_selectItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._COMMA_selectItem(_COMMA_1.Parsed,  _selectItem_1.Parsed), _selectItem_1.Remainder);
            }
        }
    }
    
}
