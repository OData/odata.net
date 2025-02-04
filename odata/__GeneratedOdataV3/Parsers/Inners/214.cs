namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_selectOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption> Parse(IInput<char>? input)
            {
                var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(input);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption)!, input);
}

var _selectOption_1 = __GeneratedOdataV3.Parsers.Rules._selectOptionParser.Instance.Parse(_SEMI_1.Remainder);
if (!_selectOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOption(_SEMI_1.Parsed, _selectOption_1.Parsed), _selectOption_1.Remainder);
            }
        }
    }
    
}
