namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption> Parse(IInput<char>? input)
            {
                var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(input);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption)!, input);
}

var _expandOption_1 = __GeneratedOdataV3.Parsers.Rules._expandOptionParser.Instance.Parse(_SEMI_1.Remainder);
if (!_expandOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption(_SEMI_1.Parsed, _expandOption_1.Parsed), _expandOption_1.Remainder);
            }
        }
    }
    
}
