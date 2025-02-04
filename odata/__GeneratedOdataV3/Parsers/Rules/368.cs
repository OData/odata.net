namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _OWSParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._OWS> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._OWS>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._OWS> Parse(IInput<char>? input)
            {
                var _ⲤSPⳆHTABↃ_1 = Inners._ⲤSPⳆHTABↃParser.Instance.Many().Parse(input);
if (!_ⲤSPⳆHTABↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._OWS)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._OWS(_ⲤSPⳆHTABↃ_1.Parsed), _ⲤSPⳆHTABↃ_1.Remainder);
            }
        }
    }
    
}
