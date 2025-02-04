namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandCountOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption> Parse(IInput<char>? input)
            {
                var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(input);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption)!, input);
}

var _expandCountOption_1 = __GeneratedOdataV3.Parsers.Rules._expandCountOptionParser.Instance.Parse(_SEMI_1.Remainder);
if (!_expandCountOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption(_SEMI_1.Parsed, _expandCountOption_1.Parsed), _expandCountOption_1.Remainder);
            }
        }
    }
    
}
