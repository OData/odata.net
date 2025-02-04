namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectList> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectList>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectList> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectList)!, input);
}

var _selectListItem_1 = __GeneratedOdataV3.Parsers.Rules._selectListItemParser.Instance.Parse(_OPEN_1.Remainder);
if (!_selectListItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectList)!, input);
}

var _ⲤCOMMA_selectListItemↃ_1 = Inners._ⲤCOMMA_selectListItemↃParser.Instance.Many().Parse(_selectListItem_1.Remainder);
if (!_ⲤCOMMA_selectListItemↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectList)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_selectListItemↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectList)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectList(_OPEN_1.Parsed, _selectListItem_1.Parsed, _ⲤCOMMA_selectListItemↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
