namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pcharⲻnoⲻSQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE> Instance { get; } = _pctⲻencodedⲻnoⲻSQUOTEParser.Instance.Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_unreservedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx26ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx3DʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE>(_ʺx40ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV3.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedⲻnoⲻSQUOTEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE> Parse(IInput<char>? input)
                {
                    var _pctⲻencodedⲻnoⲻSQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._pctⲻencodedⲻnoⲻSQUOTEParser.Instance.Parse(input);
if (!_pctⲻencodedⲻnoⲻSQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._pctⲻencodedⲻnoⲻSQUOTE(_pctⲻencodedⲻnoⲻSQUOTE_1.Parsed), _pctⲻencodedⲻnoⲻSQUOTE_1.Remainder);
                }
            }
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims> Parse(IInput<char>? input)
                {
                    var _otherⲻdelims_1 = __GeneratedOdataV3.Parsers.Rules._otherⲻdelimsParser.Instance.Parse(input);
if (!_otherⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._otherⲻdelims(_otherⲻdelims_1.Parsed), _otherⲻdelims_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24ʺParser.Instance.Parse(input);
if (!_ʺx24ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx24ʺ.Instance, _ʺx24ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx26ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx26ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx26ʺ.Instance, _ʺx26ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Dʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3DʺParser.Instance.Parse(input);
if (!_ʺx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Dʺ.Instance, _ʺx3Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx40ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx40ʺParser.Instance.Parse(input);
if (!_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Rules._pcharⲻnoⲻSQUOTE._ʺx40ʺ.Instance, _ʺx40ʺ_1.Remainder);
                }
            }
        }
    }
    
}
