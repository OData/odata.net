namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_selectOptionPCParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC> Parse(IInput<char>? input)
            {
                var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(input);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC)!, input);
}

var _selectOptionPC_1 = __GeneratedOdataV3.Parsers.Rules._selectOptionPCParser.Instance.Parse(_SEMI_1.Remainder);
if (!_selectOptionPC_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SEMI_selectOptionPC(_SEMI_1.Parsed, _selectOptionPC_1.Parsed), _selectOptionPC_1.Remainder);
            }
        }
    }
    
}
