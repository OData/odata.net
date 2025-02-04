namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ> Instance { get; } = (_SPParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_HTABParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_ʺx25x32x30ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ>(_ʺx25x30x39ʺParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP> Parse(IInput<char>? input)
                {
                    var _SP_1 = __GeneratedOdataV3.Parsers.Rules._SPParser.Instance.Parse(input);
if (!_SP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP.Instance, _SP_1.Remainder);
                }
            }
        }
        
        public static class _HTABParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB> Parse(IInput<char>? input)
                {
                    var _HTAB_1 = __GeneratedOdataV3.Parsers.Rules._HTABParser.Instance.Parse(input);
if (!_HTAB_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._HTAB.Instance, _HTAB_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x32x30ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x32x30ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x32x30ʺParser.Instance.Parse(input);
if (!_ʺx25x32x30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x32x30ʺ.Instance, _ʺx25x32x30ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25x30x39ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx25x30x39ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25x30x39ʺParser.Instance.Parse(input);
if (!_ʺx25x30x39ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._ʺx25x30x39ʺ.Instance, _ʺx25x30x39ʺ_1.Remainder);
                }
            }
        }
    }
    
}
