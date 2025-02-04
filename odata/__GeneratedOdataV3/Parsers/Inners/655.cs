namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _pcharⳆʺx2FʺⳆʺx3FʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ> Instance { get; } = (_pcharParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ>(_ʺx3FʺParser.Instance);
        
        public static class _pcharParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar> Parse(IInput<char>? input)
                {
                    var _pchar_1 = __GeneratedOdataV3.Parsers.Rules._pcharParser.Instance.Parse(input);
if (!_pchar_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._pchar(_pchar_1.Parsed), _pchar_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx2Fʺ.Instance, _ʺx2Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._pcharⳆʺx2FʺⳆʺx3Fʺ._ʺx3Fʺ.Instance, _ʺx3Fʺ_1.Remainder);
                }
            }
        }
    }
    
}
