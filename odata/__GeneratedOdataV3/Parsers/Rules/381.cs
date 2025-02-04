namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _OPENParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._OPEN> Instance { get; } = (_ʺx28ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._OPEN>(_ʺx25x32x38ʺParser.Instance);
        
        public static class _ʺx28ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx28ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx28ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx28ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx28ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx28ʺParser.Instance.Parse(input);
if (!_ʺx28ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx28ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx28ʺ(_ʺx28ʺ_1.Parsed), _ʺx28ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x38ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx25x32x38ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx25x32x38ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx25x32x38ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x38ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x38ʺParser.Instance.Parse(input);
if (!_ʺx25x32x38ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx25x32x38ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._OPEN._ʺx25x32x38ʺ(_ʺx25x32x38ʺ_1.Parsed), _ʺx25x32x38ʺ_1.Remainder);
                }
            }
        }
    }
    
}
