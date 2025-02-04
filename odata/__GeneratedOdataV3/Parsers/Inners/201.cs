namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_DIGITParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_COMMAParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_ʺx2EʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded>(_pctⲻencodedParser.Instance);
        
        public static class _ALPHAParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdataV3.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ALPHA(_ALPHA_1.Parsed), _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT> Parse(IInput<char>? input)
                {
                    var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(input);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._DIGIT(_DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _COMMAParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA> Parse(IInput<char>? input)
                {
                    var _COMMA_1 = __GeneratedOdataV3.Parsers.Rules._COMMAParser.Instance.Parse(input);
if (!_COMMA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._COMMA(_COMMA_1.Parsed), _COMMA_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2EʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(input);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._ʺx2Eʺ.Instance, _ʺx2Eʺ_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded> Parse(IInput<char>? input)
                {
                    var _pctⲻencoded_1 = __GeneratedOdataV3.Parsers.Rules._pctⲻencodedParser.Instance.Parse(input);
if (!_pctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded._pctⲻencoded(_pctⲻencoded_1.Parsed), _pctⲻencoded_1.Remainder);
                }
            }
        }
    }
    
}
