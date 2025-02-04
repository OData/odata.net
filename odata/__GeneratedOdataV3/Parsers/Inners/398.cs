namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2FʺⳆʺx25x32x46ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ> Instance { get; } = (_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ>(_ʺx25x32x46ʺParser.Instance);
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx2Fʺ(_ʺx2Fʺ_1.Parsed), _ʺx2Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x46ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x46ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x46ʺParser.Instance.Parse(input);
if (!_ʺx25x32x46ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2FʺⳆʺx25x32x46ʺ._ʺx25x32x46ʺ(_ʺx25x32x46ʺ_1.Parsed), _ʺx25x32x46ʺ_1.Remainder);
                }
            }
        }
    }
    
}
