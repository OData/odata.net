namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx3FʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR>(_ʺx27ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV4.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded> Parse(IInput<char>? input)
                {
                    var _pctⲻencoded_1 = __GeneratedOdataV4.Parsers.Rules._pctⲻencodedParser.Instance.Parse(input);
if (!_pctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._pctⲻencoded(_pctⲻencoded_1.Parsed), _pctⲻencoded_1.Remainder);
                }
            }
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims> Parse(IInput<char>? input)
                {
                    var _otherⲻdelims_1 = __GeneratedOdataV4.Parsers.Rules._otherⲻdelimsParser.Instance.Parse(input);
if (!_otherⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._otherⲻdelims(_otherⲻdelims_1.Parsed), _otherⲻdelims_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx2Fʺ.Instance, _ʺx2Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx3Fʺ.Instance, _ʺx3Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx27ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx27ʺParser.Instance.Parse(input);
if (!_ʺx27ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR._ʺx27ʺ.Instance, _ʺx27ʺ_1.Remainder);
                }
            }
        }
    }
    
}
