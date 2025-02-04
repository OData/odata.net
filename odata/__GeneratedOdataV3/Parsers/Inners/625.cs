namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_DIGITParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2BʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2DʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ>(_ʺx2EʺParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdataV3.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ALPHA(_ALPHA_1.Parsed), _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2BʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2BʺParser.Instance.Parse(input);
if (!_ʺx2Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Bʺ.Instance, _ʺx2Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2DʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2DʺParser.Instance.Parse(input);
if (!_ʺx2Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Dʺ.Instance, _ʺx2Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2EʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(input);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ._ʺx2Eʺ.Instance, _ʺx2Eʺ_1.Remainder);
                }
            }
        }
    }
    
}
