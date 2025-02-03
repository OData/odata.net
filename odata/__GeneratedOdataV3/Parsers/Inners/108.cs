namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SEMI_expandRefOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption> Parse(IInput<char>? input)
            {
                var _SEMI_1 = __GeneratedOdataV3.Parsers.Rules._SEMIParser.Instance.Parse(input);
if (!_SEMI_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption)!, input);
}

var _expandRefOption_1 = __GeneratedOdataV3.Parsers.Rules._expandRefOptionParser.Instance.Parse(_SEMI_1.Remainder);
if (!_expandRefOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._SEMI_expandRefOption(_SEMI_1.Parsed,  _expandRefOption_1.Parsed), _expandRefOption_1.Remainder);
            }
        }
    }
    
}
