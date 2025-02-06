namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_selectListItemParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem> Parse(IInput<char>? input)
            {
                var _COMMA_1 = __GeneratedOdataV4.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem)!, input);
}

var _selectListItem_1 = __GeneratedOdataV4.Parsers.Rules._selectListItemParser.Instance.Parse(_COMMA_1.Remainder);
if (!_selectListItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._COMMA_selectListItem(_COMMA_1.Parsed, _selectListItem_1.Parsed), _selectListItem_1.Remainder);
            }
        }
    }
    
}
