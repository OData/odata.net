namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmarkParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark> Instance { get; } = (_DQUOTEParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark>(_ʺx25x32x32ʺParser.Instance);
        
        public static class _DQUOTEParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._DQUOTE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._DQUOTE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._DQUOTE> Parse(IInput<char>? input)
                {
                    var _DQUOTE_1 = __GeneratedOdataV4.Parsers.Rules._DQUOTEParser.Instance.Parse(input);
if (!_DQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._DQUOTE)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._DQUOTE.Instance, _DQUOTE_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x32ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x32ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x32x32ʺParser.Instance.Parse(input);
if (!_ʺx25x32x32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._quotationⲻmark._ʺx25x32x32ʺ.Instance, _ʺx25x32x32ʺ_1.Remainder);
                }
            }
        }
    }
    
}
