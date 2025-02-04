namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _h16Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._h16> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._h16>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._h16> Parse(IInput<char>? input)
            {
                var _HEXDIG_1 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Repeat(1, 4).Parse(input);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._h16)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._h16(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedFrom1To4<__GeneratedOdataV3.CstNodes.Rules._HEXDIG>(_HEXDIG_1.Parsed)), _HEXDIG_1.Remainder);
            }
        }
    }
    
}
