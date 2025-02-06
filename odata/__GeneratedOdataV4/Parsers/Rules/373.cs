namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ATParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._AT> Instance { get; } = (_ʺx40ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._AT>(_ʺx25x34x30ʺParser.Instance);
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx40ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx40ʺParser.Instance.Parse(input);
if (!_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx40ʺ.Instance, _ʺx40ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x34x30ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x34x30ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx25x34x30ʺParser.Instance.Parse(input);
if (!_ʺx25x34x30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._AT._ʺx25x34x30ʺ.Instance, _ʺx25x34x30ʺ_1.Remainder);
                }
            }
        }
    }
    
}
